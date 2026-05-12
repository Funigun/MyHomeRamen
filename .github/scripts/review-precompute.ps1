#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Precomputes review input bundle so a cheaper review model can focus on
  semantic decisions instead of mechanical greps.

.DESCRIPTION
  Builds ./.agent-run/<run-id>/review-input.md from:
    - plan.approved.md (verbatim)
    - implementation/diff.patch with out-of-scope files stripped
    - structured greps over changed Features/ files (auth, rate-limit, manual
      tenant Where, forbidden class shapes)
    - the failure tail / overall row from verify-report.md
  Exit codes:
    0 = OK
    3 = invalid args / missing inputs
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $RunId
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$runDir   = Join-Path $repoRoot ".agent-run/$RunId"
$plan     = Join-Path $runDir 'plan.approved.md'
$diff     = Join-Path $runDir 'implementation/diff.patch'
$verify   = Join-Path $runDir 'verify-report.md'
$out      = Join-Path $runDir 'review-input.md'

foreach ($p in @($plan, $diff)) {
    if (-not (Test-Path $p)) {
        Write-Host "[review-precompute] missing input: $p"
        exit 3
    }
}

# ── Scope-allow filter (mirror of repo-context.md §5 DENY list) ──────
$denyPatterns = @(
    '^\.github/workflows/',
    '^\.github/agents/',
    '^\.claude/agents/',
    '^Directory\..*\.props$',
    '^global\.json$',
    '^BookSlot\.slnx$',
    '^\.editorconfig$',
    '^coverlet\.runsettings$'
)
function Test-InScope([string]$path) {
    foreach ($rx in $denyPatterns) {
        if ($path -match $rx) { return $false }
    }
    return $true
}

# ── Parse diff.patch into per-file blocks ────────────────────────────
$diffRaw = Get-Content $diff -Raw
$blocks = @()
$lines = $diffRaw -split "`r?`n"
$cur = $null
foreach ($ln in $lines) {
    if ($ln -match '^diff --git a/(.+?) b/(.+)$') {
        if ($cur) { $blocks += ,$cur }
        $cur = [ordered]@{ Path = $Matches[2]; Lines = New-Object System.Collections.Generic.List[string] }
    }
    if ($cur) { $cur.Lines.Add($ln) | Out-Null }
}
if ($cur) { $blocks += ,$cur }

$kept = @(); $stripped = @()
foreach ($b in $blocks) {
    if (Test-InScope $b.Path) { $kept += ,$b } else { $stripped += $b.Path }
}

$filteredDiff = ($kept | ForEach-Object { $_.Lines -join "`n" }) -join "`n"

# Files changed in scope (working-tree paths to grep)
$changedFiles = $kept | ForEach-Object { $_.Path } | Sort-Object -Unique
$featuresFiles = $changedFiles | Where-Object { $_ -like 'src/BookSlot.Features/*' -and $_ -like '*.cs' }

# ── Structured greps over changed working-tree files ─────────────────
function Grep-Lines([string]$pattern, [string[]]$paths, [string]$label) {
    $hits = @()
    foreach ($p in $paths) {
        $full = Join-Path $repoRoot $p
        if (-not (Test-Path $full)) { continue }
        $matchObjs = Select-String -Path $full -Pattern $pattern -CaseSensitive:$false -ErrorAction SilentlyContinue
        foreach ($m in $matchObjs) {
            $hits += "$p`:$($m.LineNumber): $($m.Line.Trim())"
        }
    }
    if ($hits.Count -eq 0) { return "_(no matches for $label)_" }
    return ($hits -join "`n")
}

$grepAuth   = Grep-Lines 'RequireAuthorization|AllowAnonymous|RequireRateLimiting' $featuresFiles 'auth/rate-limit'
$grepTenant = Grep-Lines 'Where\s*\(.*TenantId'                                     $featuresFiles 'manual tenant Where'
$grepShape  = Grep-Lines 'class\s+\w*(Service|Repository)\b'                        $featuresFiles 'forbidden class shape'

# ── Verify report tail ───────────────────────────────────────────────
$verifySection = if (Test-Path $verify) {
    $vr = Get-Content $verify -Raw
    $tail = ($vr -split "`r?`n") | Select-Object -Last 80
    "Last 80 lines of verify-report.md:`n`n" + (($tail) -join "`n")
} else {
    "_(verify-report.md not found — reviewer should refuse if verifier did not run)_"
}

# ── Render review-input.md ───────────────────────────────────────────
$strippedNote = if ($stripped.Count -gt 0) {
    "**Files stripped (out of scope):** " + ($stripped -join ', ')
} else { "_(no out-of-scope files in diff)_" }

$body = @"
# Review input — $RunId

This bundle is precomputed by ``scripts/review-precompute.ps1`` so the reviewer can focus on semantic judgment. **Only flag findings inside the scope of plan.approved.md.**

## Plan (approved)

$(Get-Content $plan -Raw)

---

## Diff (in-scope only)

$strippedNote

``````diff
$filteredDiff
``````

---

## Structured greps over changed Features/ files

### Auth / rate-limit annotations

``````
$grepAuth
``````

If a public endpoint is added in this diff, it MUST appear here with ``RequireAuthorization`` or explicit ``AllowAnonymous``. Auth-sensitive surface (login/registration/2FA/password/refresh) MUST also have ``RequireRateLimiting("auth-sensitive")`` on this list.

### Manual tenant ``Where`` (forbidden — global query filter handles it)

``````
$grepTenant
``````

Any non-empty result here is a **blocking** finding unless paired with ``IgnoreQueryFilters()`` and a justifying comment.

### Forbidden class shapes inside Features/ (Service / Repository)

``````
$grepShape
``````

Any match here is a **blocking** VSA violation.

---

## Verifier output

$verifySection
"@

[System.IO.File]::WriteAllText($out, $body, [System.Text.UTF8Encoding]::new($false))

Write-Host "[review-precompute] run-id: $RunId"
Write-Host "[review-precompute] in-scope files: $($changedFiles.Count); stripped: $($stripped.Count)"
Write-Host "[review-precompute] features-cs files greppable: $($featuresFiles.Count)"
Write-Host "[review-precompute] output: $out"
exit 0
