#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Verifier-as-script for the BookSlot agentic pipeline.

.DESCRIPTION
    Replaces the LLM verifier agent with a deterministic implementation:
      1. Pre-checks (test-completeness, migration-check, scope-leak).
      2. dotnet build + unit + arch tests; integration tests when plan demands them.
      3. Renders ./.agent-run/<RunId>/verify-report.md.

    Operates only on artifacts under ./.agent-run/<RunId>/ and the local repo
    working tree. Writes nothing outside the run folder.

.PARAMETER RunId
    Run identifier matching the folder under ./.agent-run/.

.PARAMETER PlanPath
    Path to the approved plan. Defaults to ./.agent-run/<RunId>/plan.approved.md.

.PARAMETER DiffPath
    Path to the implementer diff. Defaults to ./.agent-run/<RunId>/implementation/diff.patch.

.PARAMETER Iteration
    Iteration number (used only in the report header). Defaults to 1.

.OUTPUTS
    Exit 0 = PASS, 1 = FAIL, 2 = SCOPE_VIOLATION, 3 = invalid arguments.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $RunId,
    [string] $PlanPath,
    [string] $DiffPath,
    [int]    $Iteration = 1
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$runRoot  = Join-Path $repoRoot ".agent-run/$RunId"

if (-not (Test-Path $runRoot)) {
    Write-Error "Run folder not found: $runRoot"
    exit 3
}
if (-not $PlanPath) { $PlanPath = Join-Path $runRoot 'plan.approved.md' }
if (-not $DiffPath) { $DiffPath = Join-Path $runRoot 'implementation/diff.patch' }
if (-not (Test-Path $PlanPath)) { Write-Error "Plan not found: $PlanPath"; exit 3 }
if (-not (Test-Path $DiffPath)) { Write-Error "Diff not found: $DiffPath"; exit 3 }

$reportPath = Join-Path $runRoot 'verify-report.md'
$results    = [ordered]@{}
$failureNotes = @()
$fence = '```'

# ---------- Helpers ------------------------------------------------------------

function New-CheckResult {
    param([string]$Name, [string]$Status, [string]$Details = '-', [double]$DurationSec = 0)
    return [pscustomobject]@{
        Name        = $Name
        Status      = $Status
        Details     = $Details
        DurationSec = [math]::Round($DurationSec, 2)
    }
}

function Get-PlanCreatedPaths {
    param([string]$Plan)
    # Parses §3 table rows of the form: | path | create | rationale |
    # Returns lower-cased forward-slash paths whose Action column starts with "create".
    $lines = Get-Content $Plan
    $paths = New-Object System.Collections.Generic.List[string]
    foreach ($line in $lines) {
        if ($line -notmatch '^\s*\|') { continue }
        $cells = ($line -split '\|') | ForEach-Object { $_.Trim() }
        # cells[0] is empty (leading |), cells[1]=path, cells[2]=action
        if ($cells.Count -lt 4) { continue }
        $path   = $cells[1]
        $action = $cells[2]
        if ($path -match '^[-: ]+$') { continue } # separator row
        if ($action -match '^create') {
            $paths.Add(($path -replace '\\','/').ToLowerInvariant()) | Out-Null
        }
    }
    return $paths
}

function Get-DiffAddedPaths {
    param([string]$Diff)
    # Returns paths added by the diff (status A in unified-diff form).
    $added = New-Object System.Collections.Generic.List[string]
    $lines = Get-Content $Diff
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\+\+\+ b/(.+)$' -and $i -gt 0) {
            $prev = $lines[$i - 1]
            if ($prev -match '^--- /dev/null') {
                $added.Add(($Matches[1] -replace '\\','/').ToLowerInvariant()) | Out-Null
            }
        }
    }
    return $added
}

function Get-DiffAllPaths {
    param([string]$Diff)
    # Returns every file path touched in the diff (added/modified/deleted/renamed targets).
    $all = New-Object System.Collections.Generic.HashSet[string]
    foreach ($line in Get-Content $Diff) {
        if ($line -match '^diff --git a/(.+) b/(.+)$') {
            [void]$all.Add(($Matches[1] -replace '\\','/').ToLowerInvariant())
            [void]$all.Add(($Matches[2] -replace '\\','/').ToLowerInvariant())
        }
    }
    return @($all)
}

function Test-PathDeniedForImplementer {
    # DEPRECATED: kept for backward compat with any external callers.
    # Real check now lives in scripts/scope-check.ps1.
    param([string]$Path)
    $patterns = @(
        '^\.github/workflows/',
        '^\.github/agents/',
        '^\.claude/agents/',
        '^directory\..*\.props$',
        '^global\.json$',
        '^bookslot\.slnx$',
        '^\.editorconfig$',
        '^coverlet\.runsettings$'
    )
    foreach ($p in $patterns) {
        if ($Path -match $p) { return $true }
    }
    return $false
}

function Invoke-Step {
    param([string]$Name, [scriptblock]$Block)
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $output = ''
    $exit = 0
    try {
        $output = & $Block 2>&1 | Out-String
        $exit = $LASTEXITCODE
        if ($null -eq $exit) { $exit = 0 }
    } catch {
        $output += "`n$($_.Exception.Message)"
        $exit = 1
    }
    $sw.Stop()
    return [pscustomobject]@{
        Output      = $output
        ExitCode    = $exit
        DurationSec = $sw.Elapsed.TotalSeconds
    }
}

# ---------- Pre-checks ---------------------------------------------------------

$plannedAll  = Get-PlanCreatedPaths -Plan $PlanPath
$plannedTests = @($plannedAll | Where-Object { $_ -like 'tests/*' })
$diffAdded   = Get-DiffAddedPaths -Diff $DiffPath
$diffAll     = Get-DiffAllPaths   -Diff $DiffPath

# Pre-1: test completeness
$missingTests = @($plannedTests | Where-Object { $_ -notin $diffAdded })
if ($missingTests.Count -eq 0) {
    $results['Pre: test completeness'] = New-CheckResult 'Pre: test completeness' 'PASS' '-'
} else {
    $results['Pre: test completeness'] = New-CheckResult 'Pre: test completeness' 'FAIL' ("missing: " + ($missingTests -join ', '))
    $failureNotes += "Plan §3 calls for these test files but the diff does not add them:`n  " + ($missingTests -join "`n  ")
}

# Pre-2: migration check
$planText = Get-Content $PlanPath -Raw
$migrationPlanned = $planText -match 'dotnet ef migrations add'
$migrationFiles = @($diffAdded | Where-Object { $_ -like 'src/bookslot.infrastructure/persistence/migrations/*' })
if (-not $migrationPlanned) {
    $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'N/A' '-'
} elseif ($migrationFiles.Count -gt 0) {
    $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'PASS' ("found: " + ($migrationFiles -join ', '))
} else {
    $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'FAIL' 'plan §5 mentions a migration but no file under src/BookSlot.Infrastructure/Persistence/Migrations/ in diff'
    $failureNotes += 'Plan §5 calls for `dotnet ef migrations add ...` but the diff has no Persistence/Migrations/* file.'
}

# Pre-3: scope leak — delegated to scripts/scope-check.ps1 (single source of truth).
$scopeScript = Join-Path $PSScriptRoot 'scope-check.ps1'
$leakedPaths = @()
$scopeOutput = & pwsh -NoProfile -File $scopeScript -Agent implementer -Diff $DiffPath -Quiet 2>&1
$scopeExit = $LASTEXITCODE
if ($scopeExit -eq 0) {
    $results['Pre: scope leak'] = New-CheckResult 'Pre: scope leak' 'PASS' '-'
    $scopeLeak = $false
} else {
    $scopeLeak = $true
    $leakedPaths = @($scopeOutput | Where-Object { $_ -match '^\s+!\s+(.+)$' } | ForEach-Object { ($_ -replace '^\s+!\s+','').Trim() })
    $results['Pre: scope leak'] = New-CheckResult 'Pre: scope leak' 'FAIL' ("denied paths: " + ($leakedPaths -join ', '))
    $failureNotes += "Implementer touched paths outside its scope-allow:`n  " + ($leakedPaths -join "`n  ")
}

$preFailed = ($results.Values | Where-Object { $_.Status -eq 'FAIL' }).Count -gt 0

# ---------- Build / test (skipped on pre-fail) --------------------------------

$skippedBecauseOfPre = $preFailed
$buildRun = $null

if (-not $skippedBecauseOfPre) {
    Push-Location $repoRoot
    try {
        $buildRun = Invoke-Step 'dotnet build' { dotnet build BookSlot.slnx --nologo }
        $status = if ($buildRun.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
        $results['dotnet build'] = New-CheckResult 'dotnet build' $status '-' $buildRun.DurationSec

        if ($buildRun.ExitCode -ne 0) {
            $failureNotes += "Build failed; skipping tests."
            $tail = ($buildRun.Output -split "`n") | Select-Object -Last 80
            $failureNotes += ("Build output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
            $results['Unit tests']         = New-CheckResult 'Unit tests'         'SKIPPED'
            $results['Architecture tests'] = New-CheckResult 'Architecture tests' 'SKIPPED'
            $results['Integration tests']  = New-CheckResult 'Integration tests'  'SKIPPED'
        } else {
            $unit = Invoke-Step 'unit' { dotnet test tests/BookSlot.UnitTests/BookSlot.UnitTests.csproj --nologo --no-build }
            $unitStatus = if ($unit.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
            $results['Unit tests'] = New-CheckResult 'Unit tests' $unitStatus '-' $unit.DurationSec
            if ($unit.ExitCode -ne 0) {
                $tail = ($unit.Output -split "`n") | Select-Object -Last 80
                $failureNotes += ("Unit tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
            }

            $arch = Invoke-Step 'arch' { dotnet test tests/BookSlot.ArchitectureTests/BookSlot.ArchitectureTests.csproj --nologo --no-build }
            $archStatus = if ($arch.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
            $results['Architecture tests'] = New-CheckResult 'Architecture tests' $archStatus '-' $arch.DurationSec
            if ($arch.ExitCode -ne 0) {
                $tail = ($arch.Output -split "`n") | Select-Object -Last 80
                $failureNotes += ("Arch tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
            }

            $needsIntegration = ($plannedAll | Where-Object { $_ -like 'tests/bookslot.integrationtests/*' }).Count -gt 0
            if ($needsIntegration) {
                $itg = Invoke-Step 'integration' { dotnet test tests/BookSlot.IntegrationTests/BookSlot.IntegrationTests.csproj --nologo --no-build }
                $itgStatus = if ($itg.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
                $results['Integration tests'] = New-CheckResult 'Integration tests' $itgStatus '-' $itg.DurationSec
                if ($itg.ExitCode -ne 0) {
                    $tail = ($itg.Output -split "`n") | Select-Object -Last 80
                    $failureNotes += ("Integration tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
                }
            } else {
                $results['Integration tests'] = New-CheckResult 'Integration tests' 'SKIPPED' 'plan does not include tests/BookSlot.IntegrationTests/*'
            }
        }
    } finally {
        Pop-Location
    }
} else {
    foreach ($k in 'dotnet build','Unit tests','Architecture tests','Integration tests') {
        $results[$k] = New-CheckResult $k 'SKIPPED' 'pre-check failed'
    }
}

# ---------- Render report ------------------------------------------------------

$overallFail = ($results.Values | Where-Object { $_.Status -eq 'FAIL' }).Count -gt 0
$overall = if ($scopeLeak) { 'SCOPE_VIOLATION' } elseif ($overallFail) { 'FAIL' } else { 'PASS' }

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# Verify report — iteration $Iteration")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Check | Result | Duration | Details |")
[void]$sb.AppendLine("|-------|--------|----------|---------|")
foreach ($r in $results.Values) {
    $dur = if ($r.DurationSec -gt 0) { "$($r.DurationSec)s" } else { '-' }
    [void]$sb.AppendLine("| $($r.Name) | $($r.Status) | $dur | $($r.Details) |")
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Overall: $overall")
[void]$sb.AppendLine("")
if ($failureNotes.Count -gt 0) {
    [void]$sb.AppendLine("## Failure tail")
    foreach ($n in $failureNotes) {
        [void]$sb.AppendLine($n)
        [void]$sb.AppendLine("")
    }
}

Set-Content -Path $reportPath -Value $sb.ToString() -Encoding utf8

# ---------- Stdout summary -----------------------------------------------------

$summary = $results.Values |
    ForEach-Object { "$($_.Name)=$($_.Status)" } |
    Join-String -Separator ' '
Write-Host "[verifier] iter $Iteration`: $summary"
Write-Host "[verifier] overall: $overall"
Write-Host "[verifier] report: $reportPath"

switch ($overall) {
    'PASS'            { exit 0 }
    'SCOPE_VIOLATION' { exit 2 }
    default           { exit 1 }
}
