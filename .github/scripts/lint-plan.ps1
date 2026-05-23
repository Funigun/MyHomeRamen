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
  pwsh .github/scripts/lint-plan.ps1 -PlanPath .github/plans/menu/add-category-plan-backend.md
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

# ── 2. Required sections 1..7 ────────────────────────────────────────
$expected = @(
    @{ N = 1; Hint = 'Problem' },
    @{ N = 2; Hint = 'Files to create / modify' },
    @{ N = 3; Hint = 'Domain changes' },
    @{ N = 4; Hint = 'API details' },
    @{ N = 5; Hint = 'Tests' },
    @{ N = 6; Hint = 'Risks' },
    @{ N = 7; Hint = 'Out of scope' }
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
    foreach ($k in ($n + 1)..8) {
        if ($sectionStart.ContainsKey($k)) { $end = $sectionStart[$k] - 1; break }
    }
    return $lines[$start..$end]
}

# ── 3. §2 files table structure ──────────────────────────────────────
if ($sectionStart.ContainsKey(2)) {
    $sec2 = Get-SectionLines 2
    $tableLines = $sec2 | Where-Object { $_ -match '^\s*\|' }
    if ($tableLines.Count -lt 3) {
        Add-Issue "§2 must contain a markdown table with header + divider + at least one row (got $($tableLines.Count) line(s))."
    } else {
        $headerCols = ($tableLines[0] -split '\|') | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        $needed = @('Path', 'Action', 'Type')
        $missing = $needed | Where-Object { $h = $_; -not ($headerCols | Where-Object { $_ -ieq $h }) }
        if ($missing) {
            Add-Issue "§2 table header must contain columns 'Path | Action | Type' (missing: $($missing -join ', '))."
        }
        # Validate body rows
        $bodyRows = $tableLines | Select-Object -Skip 2
        $rowIdx = 0
        foreach ($row in $bodyRows) {
            $rowIdx++
            # Split on pipe; leading/trailing pipes produce empty first/last entries — skip them
            $allCols = ($row -split '\|') | ForEach-Object { $_.Trim() }
            $allCols = $allCols[1..($allCols.Count - 2)]   # drop leading/trailing empty entries
            if ($allCols.Count -lt 2 -or -not $allCols[0] -or -not $allCols[1]) {
                Add-Issue "§2 table row #$rowIdx must have non-empty Path and Action columns: '$row'"
                continue
            }
            $action = $allCols[1].ToLowerInvariant()
            if ($action -notin @('create', 'modify', 'delete')) {
                Add-Issue "§2 table row #$rowIdx has unknown Action '$($allCols[1])' (expected create/modify/delete)."
            }
        }
    }
}

# ── 4. Rate-limit rule on auth-sensitive endpoints (§4 API details) ──
if ($sectionStart.ContainsKey(4)) {
    $sec4Text = (Get-SectionLines 4) -join "`n"
    $authRx = '(?i)(\blogin\b|\bsignin\b|\bsign-in\b|\bsignup\b|\bsign-up\b|\b2fa\b|\bmfa\b|\bpassword\b|\brefresh[- ]?token\b|\breset[- ]?password\b|\bforgot[- ]?password\b|/auth/)'
    if ($sec4Text -match $authRx) {
        if ($sec4Text -notmatch 'auth-sensitive') {
            Add-Issue "§4 mentions auth-sensitive surface ($($Matches[0])) but does not specify ``RequireRateLimiting(""auth-sensitive"")``."
        }
    }
}

# ── 5. §5 Tests must list at least one unit test ─────────────────────
if ($sectionStart.ContainsKey(5)) {
    $sec5Text = (Get-SectionLines 5) -join "`n"
    if ($sec5Text -notmatch '(?i)unit') {
        Add-Issue "§5 must mention at least one unit test (none found)."
    }
}

# ── 6. Module name validation ────────────────────────────────────────
$validModules = @('Users', 'Menu', 'Orders', 'ShoppingCart', 'Reservations', 'Payments')
if ($lines[0] -match '^#\s+Plan:\s+(\S[^-]+?)\s+-') {
    $planModule = $Matches[1].Trim()
    if ($planModule -notin $validModules) {
        Add-Issue "§title module '$planModule' is not a valid module. Valid modules: $($validModules -join ', ')."
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
