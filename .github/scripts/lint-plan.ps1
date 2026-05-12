#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Static lint of an agent-generated plan.md before HITL #1.

.DESCRIPTION
  Checks structural invariants of plan.md so the human reviewer doesn't waste
  attention on mechanical issues. Intended to run between planner output and
  the AWAITING_HUMAN gate. Exit code:
    0 = OK (no issues)
    1 = soft-fail (plan has issues; planner should re-pass)
    3 = invalid arguments / file missing

.PARAMETER PlanPath
  Absolute or repo-relative path to plan.md to lint. Required.

.EXAMPLE
  pwsh ./scripts/lint-plan.ps1 -PlanPath ./.agent-run/abc123/plan.md
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $PlanPath
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $PlanPath)) {
    Write-Host "[lint-plan] file not found: $PlanPath"
    exit 3
}

$raw = Get-Content $PlanPath -Raw
$lines = $raw -split "`r?`n"

$issues = New-Object System.Collections.Generic.List[string]
function Add-Issue([string]$msg) { $issues.Add($msg) | Out-Null }

# ── 1. Top-level heading ─────────────────────────────────────────────
if ($lines[0] -notmatch '^#\s+Plan:\s+\S') {
    Add-Issue "missing top-level heading. First line must match '# Plan: <title>'."
}

# ── 2. Required sections 1..8 ────────────────────────────────────────
$expected = @(
    @{ N = 1; Hint = 'Problem' },
    @{ N = 2; Hint = 'Proposed approach' },
    @{ N = 3; Hint = 'Files to create / modify' },
    @{ N = 4; Hint = 'API contract' },
    @{ N = 5; Hint = 'Domain & data model' },
    @{ N = 6; Hint = 'Tests' },
    @{ N = 7; Hint = 'Risks' },
    @{ N = 8; Hint = 'Out of scope' }
)
$sectionStart = @{}
foreach ($e in $expected) {
    $rx = "^##\s+$($e.N)\.\s+\S"
    $idx = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match $rx) { $idx = $i; break }
    }
    if ($idx -lt 0) {
        Add-Issue "missing section §$($e.N) (expected '## $($e.N). $($e.Hint)' or similar)."
    } else {
        $sectionStart[$e.N] = $idx
    }
}

function Get-SectionLines([int]$n) {
    if (-not $sectionStart.ContainsKey($n)) { return @() }
    $start = $sectionStart[$n] + 1
    # next §N+1 or §N+2 or end of file
    $end = $lines.Count - 1
    foreach ($k in ($n + 1)..9) {
        if ($sectionStart.ContainsKey($k)) { $end = $sectionStart[$k] - 1; break }
    }
    return $lines[$start..$end]
}

# ── 3. §3 table structure ────────────────────────────────────────────
if ($sectionStart.ContainsKey(3)) {
    $sec3 = Get-SectionLines 3
    $tableLines = $sec3 | Where-Object { $_ -match '^\s*\|' }
    if ($tableLines.Count -lt 3) {
        Add-Issue "§3 must contain a markdown table with header + divider + at least one row (got $($tableLines.Count) line(s))."
    } else {
        $headerCols = ($tableLines[0] -split '\|') | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        $needed = @('Path', 'Action', 'Rationale')
        $missing = $needed | Where-Object { $h = $_; -not ($headerCols | Where-Object { $_ -ieq $h }) }
        if ($missing) {
            Add-Issue "§3 table header must contain columns 'Path | Action | Rationale' (missing: $($missing -join ', '))."
        }
        # Validate body rows
        $bodyRows = $tableLines | Select-Object -Skip 2
        $rowIdx = 0
        foreach ($row in $bodyRows) {
            $rowIdx++
            $cols = ($row -split '\|') | ForEach-Object { $_.Trim() }
            # First and last entries from a leading/trailing pipe are empty
            $cols = $cols | Where-Object { $_ -ne '' }
            if ($cols.Count -lt 3) {
                Add-Issue "§3 table row #$rowIdx has fewer than 3 columns: '$row'"
                continue
            }
            $action = $cols[1].ToLowerInvariant()
            if ($action -notin @('create', 'modify', 'delete')) {
                Add-Issue "§3 table row #$rowIdx has unknown Action '$($cols[1])' (expected create/modify/delete)."
            }
        }
    }
}

# ── 4. Rate-limit rule on auth-sensitive endpoints ───────────────────
if ($sectionStart.ContainsKey(4)) {
    $sec4Text = (Get-SectionLines 4) -join "`n"
    $authRx = '(?i)(\blogin\b|\bsignin\b|\bsign-in\b|\bsignup\b|\bsign-up\b|\b2fa\b|\bmfa\b|\bpassword\b|\brefresh[- ]?token\b|\breset[- ]?password\b|\bforgot[- ]?password\b|/auth/)'
    if ($sec4Text -match $authRx) {
        if ($sec4Text -notmatch 'auth-sensitive') {
            Add-Issue "§4 mentions auth-sensitive surface ($($Matches[0])) but does not specify ``RequireRateLimiting(""auth-sensitive"")``."
        }
    }
}

# ── 5. §6 Tests must list at least one unit test ─────────────────────
if ($sectionStart.ContainsKey(6)) {
    $sec6Text = (Get-SectionLines 6) -join "`n"
    if ($sec6Text -notmatch '(?i)unit') {
        Add-Issue "§6 must mention at least one unit test (none found)."
    }
}

# ── Report ────────────────────────────────────────────────────────────
if ($issues.Count -eq 0) {
    Write-Host "[lint-plan] OK — $PlanPath"
    exit 0
}

Write-Host "[lint-plan] $($issues.Count) issue(s) in $PlanPath"
$i = 0
foreach ($msg in $issues) {
    $i++
    Write-Host "  $i. $msg"
}
exit 1
