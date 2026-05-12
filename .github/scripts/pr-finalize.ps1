<#
.SYNOPSIS
  Deterministic PR finalization. Runs all the mechanical git/gh/diff steps that
  used to live inside the pr-commit LLM agent. Leaves only the
  agent-decisions.md authoring to the LLM (which receives the deltas this script
  produces).

.PARAMETER RunId
  The agent-run identifier. The run dir is ./.agent-run/<RunId>/.

.PARAMETER SkipPush
  Smoke / dry-run flag — don't push or open a PR; useful for tests.

.NOTES
  Exit codes:
    0  finalize succeeded (or smoke walked through)
    2  preconditions not met (review not approved, verifier not PASS, ...)
    3  invalid args / missing run dir
    4  git/gh failure
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$RunId,
    [switch]$SkipPush
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path "$PSScriptRoot/..").Path
$runDir   = Join-Path $repoRoot ".agent-run/$RunId"

if (-not (Test-Path $runDir)) {
    Write-Error "[pr-finalize] run dir not found: $runDir"
    exit 3
}

function Read-IfExists([string]$path) {
    if (Test-Path $path) { Get-Content $path -Raw } else { '' }
}

# --- Preconditions --------------------------------------------------------
$planPath        = Join-Path $runDir 'plan.md'
$planApprovedPath= Join-Path $runDir 'plan.approved.md'
$reviewPath      = Join-Path $runDir 'review.md'
$reviewApproved  = Join-Path $runDir 'review.approved.md'
$verifyReport    = Join-Path $runDir 'verify-report.md'
$prBody          = Join-Path $runDir 'pr-body.md'

if (-not (Test-Path $planApprovedPath)) {
    Write-Error "[pr-finalize] plan.approved.md missing"; exit 2
}
if (-not (Test-Path $reviewApproved)) {
    Write-Error "[pr-finalize] review.approved.md missing"; exit 2
}
$reviewText = Read-IfExists $reviewApproved
if ($reviewText -match '(?im)^\s*ABORT\b') {
    Write-Error "[pr-finalize] review.approved.md = ABORT"; exit 2
}
if ($reviewText -match '(?im)^\s*REQUEST_CHANGES\b') {
    Write-Error "[pr-finalize] review.approved.md = REQUEST_CHANGES (back to implementer)"; exit 2
}
$verifyText = Read-IfExists $verifyReport
if (-not $verifyText -or $verifyText -notmatch '(?im)^##\s+Overall:\s+PASS\b') {
    Write-Error "[pr-finalize] last verify-report.md is not PASS"; exit 2
}

# --- Slug from plan title -------------------------------------------------
$planText = Read-IfExists $planApprovedPath
$titleMatch = [regex]::Match($planText, '(?m)^\s*#\s+Plan:\s+(.+?)\s*$')
$planTitle = if ($titleMatch.Success) { $titleMatch.Groups[1].Value } else { "agent-run-$RunId" }
$slug = ($planTitle.ToLowerInvariant() -replace '[^a-z0-9]+','-').Trim('-')
if ([string]::IsNullOrWhiteSpace($slug)) { $slug = "agent-run-$RunId" }
$branch = "feature/$slug"

# --- Deltas (plan.md vs plan.approved.md, review.md vs review.approved.md) -
$planDelta   = Join-Path $runDir 'plan.delta.txt'
$reviewDelta = Join-Path $runDir 'review.delta.txt'

function Write-Delta([string]$a, [string]$b, [string]$out) {
    if (-not (Test-Path $a) -or -not (Test-Path $b)) {
        Set-Content -Path $out -Value "(one of the inputs is missing — no delta)" -Encoding UTF8
        return
    }
    $diff = & git --no-pager diff --no-index --no-color -- $a $b 2>&1
    if (-not $diff) { $diff = '(no differences — accepted as-is)' }
    Set-Content -Path $out -Value ($diff -join [Environment]::NewLine) -Encoding UTF8
}
Write-Delta $planPath   $planApprovedPath $planDelta
Write-Delta $reviewPath $reviewApproved   $reviewDelta

# --- Branch ---------------------------------------------------------------
Push-Location $repoRoot
try {
    $existingBranch = (& git rev-parse --abbrev-ref HEAD 2>$null).Trim()
    if ($existingBranch -ne $branch) {
        & git checkout -B $branch | Out-Null
        if ($LASTEXITCODE -ne 0) { Write-Error "[pr-finalize] git checkout failed"; exit 4 }
    }

    if ($SkipPush) {
        Write-Host "[pr-finalize] SkipPush — would commit on branch: $branch"
        Write-Host "[pr-finalize] plan delta: $planDelta"
        Write-Host "[pr-finalize] review delta: $reviewDelta"
        exit 0
    }

    # --- Commit ---------------------------------------------------------------
    & git add -A | Out-Null
    $commitMsg = @"
feat($($slug.Split('-')[0])): $planTitle

Plan: ./.agent-run/$RunId/plan.approved.md
Review: ./.agent-run/$RunId/review.approved.md
Verifier: PASS

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
"@
    & git commit -m $commitMsg | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Error "[pr-finalize] git commit failed"; exit 4 }
    $commitSha = (& git rev-parse HEAD).Trim()

    # --- Push -----------------------------------------------------------------
    & git push -u origin $branch | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Error "[pr-finalize] git push failed"; exit 4 }

    # --- PR -------------------------------------------------------------------
    if (-not (Test-Path $prBody)) {
        Set-Content -Path $prBody -Value "## Summary`n`n$planTitle" -Encoding UTF8
    }
    & gh pr create --title $planTitle --body-file $prBody --draft=false | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Error "[pr-finalize] gh pr create failed"; exit 4 }
    $prNumber = (& gh pr view --json number -q .number).Trim()
    $prUrl    = (& gh pr view --json url    -q .url   ).Trim()

    # --- CI status (best-effort, max 90s) ------------------------------------
    $deadline = (Get-Date).AddSeconds(90)
    $first = $null
    do {
        Start-Sleep -Seconds 10
        $checksRaw = & gh pr checks $prNumber --json name,state,conclusion,link 2>$null
        if ($checksRaw) {
            $checks = $checksRaw | ConvertFrom-Json
            $first = $checks | Where-Object { $_.state -notin @('PENDING','QUEUED') } | Select-Object -First 1
        }
    } until ($first -or (Get-Date) -ge $deadline)

    $ciName = if ($first) { $first.name } else { '(none yet)' }
    $ciConclusion = if ($first) { $first.conclusion } else { 'queued' }
    $ciLink = if ($first) { $first.link } else { '' }
    $ciBlock = @"

## CI status
- First check: ``$ciName`` — ``$ciConclusion`` ($ciLink)
- Captured at: $(Get-Date -Format 'yyyy-MM-ddTHH:mm:ssZ')
- Waited up to 90s after PR open. Re-check before merge.
"@
    Add-Content -Path $prBody -Value $ciBlock -Encoding UTF8
    & gh pr edit $prNumber --body-file $prBody | Out-Null

    Write-Host "[pr-finalize] branch: $branch"
    Write-Host "[pr-finalize] commit: $commitSha"
    Write-Host "[pr-finalize] PR: $prUrl"
    Write-Host "[pr-finalize] CI first check: $ciName = $ciConclusion"
    Write-Host "[pr-finalize] plan delta: $planDelta"
    Write-Host "[pr-finalize] review delta: $reviewDelta"
    Write-Host "[pr-finalize] DONE"
    exit 0
}
finally {
    Pop-Location
}
