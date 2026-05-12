<#
.SYNOPSIS
  Deterministic scope-allow / scope-deny checker for any agent.
  Replaces the inline regex in verify.ps1 with a single source of truth that
  can also be invoked from a git pre-commit hook.

.PARAMETER Agent
  Agent name. Supported: planner | implementer | verifier | code-reviewer | pr-commit | orchestrator.

.PARAMETER Diff
  Optional path to a unified diff. If omitted: uses `git diff --name-only HEAD`
  (i.e. the current working tree against the last commit).

.PARAMETER Quiet
  Suppress per-file output; still prints the summary line.

.NOTES
  Exit codes:
    0  no scope violation
    2  scope violation — see stdout for the offending paths
    3  invalid args
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet('planner','implementer','verifier','code-reviewer','pr-commit','orchestrator')]
    [string]$Agent,
    [string]$Diff,
    [switch]$Quiet
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path "$PSScriptRoot/..").Path

# --- Allow / deny rules --------------------------------------------------
# Mirrors _shared/repo-context.md §5. If you change the table there, change here too.
# Patterns are regex matched against forward-slash relative paths.
$rules = @{
    'planner' = @{
        Allow = @()                         # planner is read-only
        Deny  = @('.+')                     # writes to anything = violation
    }
    'verifier' = @{
        Allow = @()
        Deny  = @('.+')
    }
    'code-reviewer' = @{
        Allow = @()
        Deny  = @('.+')
    }
    'orchestrator' = @{
        # Orchestrator only writes to .agent-run/<run>/ via agent-run.ps1.
        Allow = @('^\.agent-run/')
        Deny  = @('.+')
    }
    'implementer' = @{
        Allow = @(
            '^src/BookSlot\.Features/',
            '^src/BookSlot\.Domain/',
            '^src/BookSlot\.Infrastructure/',
            '^tests/',
            '^docs/ARCHITECTURE\.md$',
            '^docs/RUNBOOK\.md$',
            '^\.agent-run/'
        )
        Deny  = @(
            '^\.github/workflows/',
            '^\.github/agents/',
            '^\.claude/agents/',
            '^Directory\..+\.props$',
            '^global\.json$',
            '^BookSlot\.slnx$',
            '^\.editorconfig$',
            '^coverlet\.runsettings$',
            '^docs/agent-decisions\.md$'
        )
    }
    'pr-commit' = @{
        Allow = @(
            '^docs/agent-decisions\.md$',
            '^CHANGELOG\.md$',
            '^\.agent-run/'
        )
        # pr-commit may not modify production code (implementer already did that).
        Deny  = @(
            '^src/',
            '^tests/',
            '^\.github/workflows/',
            '^\.github/agents/',
            '^\.claude/agents/',
            '^Directory\..+\.props$',
            '^global\.json$',
            '^BookSlot\.slnx$'
        )
    }
}

if (-not $rules.ContainsKey($Agent)) {
    Write-Error "[scope-check] unknown agent: $Agent"; exit 3
}
$agentRules = $rules[$Agent]

# --- Load changed files ---------------------------------------------------
function Get-ChangedFiles {
    if ($Diff -and (Test-Path $Diff)) {
        $patch = Get-Content $Diff -Raw
        $matches = [regex]::Matches($patch, '(?m)^diff --git a/(\S+) b/(\S+)')
        $files = @()
        foreach ($m in $matches) { $files += $m.Groups[2].Value }
        return $files | Sort-Object -Unique
    }
    Push-Location $repoRoot
    try {
        $tracked = & git diff --name-only HEAD 2>$null
        $untracked = & git ls-files --others --exclude-standard 2>$null
        return @($tracked + $untracked) | Where-Object { $_ } | Sort-Object -Unique
    } finally { Pop-Location }
}

$files = Get-ChangedFiles
if (-not $files -or $files.Count -eq 0) {
    if (-not $Quiet) { Write-Host "[scope-check] ${Agent}: no changed files." }
    exit 0
}

# --- Evaluate -------------------------------------------------------------
function Test-Match($path, $patterns) {
    foreach ($p in $patterns) { if ($path -match $p) { return $true } }
    return $false
}

$violations = @()
foreach ($f in $files) {
    $allowed = if ($agentRules.Allow.Count -eq 0) { $false } else { Test-Match $f $agentRules.Allow }
    $denied  = if ($agentRules.Deny.Count  -eq 0) { $false } else { Test-Match $f $agentRules.Deny  }
    # Allow always wins over deny (an explicit allow whitelists known exceptions).
    if (-not $allowed -and $denied) { $violations += $f }
}

if (-not $Quiet) {
    foreach ($f in $files) {
        $tag = if ($violations -contains $f) { 'DENY' } else { 'OK' }
        Write-Host ("  [{0,-4}] {1}" -f $tag, $f)
    }
}

if ($violations.Count -eq 0) {
    Write-Host "[scope-check] ${Agent}: $($files.Count) file(s) checked, no violations."
    exit 0
}

Write-Host "[scope-check] ${Agent}: SCOPE_VIOLATION ($($violations.Count) of $($files.Count) file(s))"
foreach ($v in $violations) { Write-Host "  ! $v" }
exit 2
