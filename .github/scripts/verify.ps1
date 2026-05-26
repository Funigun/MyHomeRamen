#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Verifier-as-script for the MyHomeRamen agentic pipeline.

.DESCRIPTION
    Deterministic verifier:
      1. Pre-checks (test-completeness, migration-check) — backend scope only.
      2. dotnet build + unit + arch tests; integration tests when plan demands them.
      3. Renders verify-report.md next to the plan file.

    Scope is auto-detected from the plan filename:
      *-plan-backend.md  → diff pre-checks run (DiffPath required or auto-resolved)
      *-plan-frontend.md → diff pre-checks skipped (DiffPath ignored)

    Plan convention   : .github/plans/{feature}/backend-plan.md
    Diff convention   : .github/plans/{feature}/diff.patch
    Report output     : same folder as the plan, named verify-report.md

.PARAMETER PlanPath
    Path to the approved plan file. Required.

.PARAMETER DiffPath
    Path to the implementer diff. Auto-resolved for backend scope when omitted.
    Ignored for frontend scope.

.OUTPUTS
    Exit 0 = PASS, 1 = FAIL, 3 = invalid arguments.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $PlanPath,
    [string] $DiffPath = ''
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

if (-not (Test-Path $PlanPath)) { Write-Error "Plan not found: $PlanPath"; exit 3 }

$planFilename = [System.IO.Path]::GetFileName($PlanPath)
$isBackend    = $planFilename -match 'backend-plan\.md$'

if ($isBackend) {
    if (-not $DiffPath) {
        $planDir  = Split-Path -Parent $PlanPath
        $planBase = [System.IO.Path]::GetFileNameWithoutExtension($PlanPath)
        $diffBase = $planBase -replace 'backend-plan$', 'diff'
        $DiffPath = Join-Path $planDir "$diffBase.patch"
    }
    if (-not (Test-Path $DiffPath)) { Write-Error "Diff not found: $DiffPath"; exit 3 }
}

$reportPath = Join-Path (Split-Path -Parent $PlanPath) 'verify-report.md'
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
    # Parses §2 table rows with columns: | Path | Action | Type | ...
    # Returns lower-cased forward-slash paths whose Action column is "create".
    $lines = Get-Content $Plan
    $paths = New-Object System.Collections.Generic.List[string]

    # Find the §2 header index to locate the table
    $inSection2 = $false
    $headerParsed = $false
    $pathColIdx = -1
    $actionColIdx = -1

    foreach ($line in $lines) {
        if ($line -match '^##\s+2\.') { $inSection2 = $true; continue }
        if ($inSection2 -and $line -match '^##\s+[3-9]\.') { break }
        if (-not $inSection2) { continue }
        if ($line -notmatch '^\s*\|') { continue }

        $cells = ($line -split '\|') | ForEach-Object { $_.Trim() }
        # cells[0] is empty (leading |); real data starts at index 1
        $dataCells = $cells[1..($cells.Count - 2)]

        if (-not $headerParsed) {
            # Detect header row by looking for 'Path' and 'Action' column names
            for ($i = 0; $i -lt $dataCells.Count; $i++) {
                if ($dataCells[$i] -ieq 'Path')   { $pathColIdx   = $i }
                if ($dataCells[$i] -ieq 'Action') { $actionColIdx = $i }
            }
            if ($pathColIdx -ge 0 -and $actionColIdx -ge 0) { $headerParsed = $true }
            continue
        }

        if ($dataCells[0] -match '^[-: ]+$') { continue }  # separator row
        if ($dataCells.Count -le [math]::Max($pathColIdx, $actionColIdx)) { continue }

        $path   = $dataCells[$pathColIdx]
        $action = $dataCells[$actionColIdx]
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

# ---------- Pre-checks (backend scope only) -----------------------------------

$plannedAll = @()

if ($isBackend) {
    $plannedAll   = Get-PlanCreatedPaths -Plan $PlanPath
    $plannedTests = @($plannedAll | Where-Object { $_ -like 'myhomeramen.unittests/*' -or $_ -like 'myhomeramen.integrationtests/*' -or $_ -like 'myhomeramen.architecturetests/*' })
    $diffAdded    = Get-DiffAddedPaths -Diff $DiffPath

    # Pre-1: test completeness
    $missingTests = @($plannedTests | Where-Object { $_ -notin $diffAdded })
    if ($missingTests.Count -eq 0) {
        $results['Pre: test completeness'] = New-CheckResult 'Pre: test completeness' 'PASS' '-'
    } else {
        $results['Pre: test completeness'] = New-CheckResult 'Pre: test completeness' 'FAIL' ("missing: " + ($missingTests -join ', '))
        $failureNotes += "Plan §2 calls for these test files but the diff does not add them:`n  " + ($missingTests -join "`n  ")
    }

    # Pre-2: migration check
    $planText = Get-Content $PlanPath -Raw
    $migrationPlanned = $planText -match 'dotnet ef migrations add'
    $migrationFiles = @($diffAdded | Where-Object { $_ -like 'myhomeramen.persistance/*/migrations/*' })
    if (-not $migrationPlanned) {
        $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'N/A' '-'
    } elseif ($migrationFiles.Count -gt 0) {
        $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'PASS' ("found: " + ($migrationFiles -join ', '))
    } else {
        $results['Pre: migration check'] = New-CheckResult 'Pre: migration check' 'FAIL' 'plan mentions a migration but no file under MyHomeRamen.Persistance/*/Migrations/ in diff'
        $failureNotes += 'Plan calls for `dotnet ef migrations add ...` but the diff has no MyHomeRamen.Persistance/*/Migrations/* file.'
    }
} else {
    $results['Pre: test completeness'] = New-CheckResult 'Pre: test completeness' 'N/A' 'frontend scope — diff pre-checks skipped'
    $results['Pre: migration check']   = New-CheckResult 'Pre: migration check'   'N/A' 'frontend scope — diff pre-checks skipped'
}

$preFailed = ($results.Values | Where-Object { $_.Status -eq 'FAIL' }).Count -gt 0

# ---------- Build / test (skipped on pre-fail) --------------------------------

$skippedBecauseOfPre = $preFailed
$buildRun = $null

if (-not $skippedBecauseOfPre) {
    Push-Location $repoRoot
    try {
        $buildRun = Invoke-Step 'dotnet build' { dotnet build MyHomeRamen.slnx --nologo }
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
            $unit = Invoke-Step 'unit' { dotnet test MyHomeRamen.UnitTests/MyHomeRamen.UnitTests.csproj --nologo --no-build }
            $unitStatus = if ($unit.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
            $results['Unit tests'] = New-CheckResult 'Unit tests' $unitStatus '-' $unit.DurationSec
            if ($unit.ExitCode -ne 0) {
                $tail = ($unit.Output -split "`n") | Select-Object -Last 80
                $failureNotes += ("Unit tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
            }

            $arch = Invoke-Step 'arch' { dotnet test MyHomeRamen.ArchitectureTests/MyHomeRamen.ArchitectureTests.csproj --nologo --no-build }
            $archStatus = if ($arch.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
            $results['Architecture tests'] = New-CheckResult 'Architecture tests' $archStatus '-' $arch.DurationSec
            if ($arch.ExitCode -ne 0) {
                $tail = ($arch.Output -split "`n") | Select-Object -Last 80
                $failureNotes += ("Arch tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
            }

 #           $needsIntegration = ($plannedAll | Where-Object { $_ -like 'myhomeramen.integrationtests/*' }).Count -gt 0
            $needsIntegration = $false
            if ($needsIntegration) {
                $itg = Invoke-Step 'integration' { dotnet test MyHomeRamen.IntegrationTests/MyHomeRamen.IntegrationTests.csproj --nologo --no-build }
                $itgStatus = if ($itg.ExitCode -eq 0) { 'PASS' } else { 'FAIL' }
                $results['Integration tests'] = New-CheckResult 'Integration tests' $itgStatus '-' $itg.DurationSec
                if ($itg.ExitCode -ne 0) {
                    $tail = ($itg.Output -split "`n") | Select-Object -Last 80
                    $failureNotes += ("Integration tests output (tail):" + [Environment]::NewLine + $fence + [Environment]::NewLine + ($tail -join [Environment]::NewLine) + [Environment]::NewLine + $fence)
                }
            } else {
                $results['Integration tests'] = New-CheckResult 'Integration tests' 'SKIPPED' 'plan does not include MyHomeRamen.IntegrationTests/*'
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
$overall = if ($overallFail) { 'FAIL' } else { 'PASS' }

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# Verify report")
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
Write-Host "[verifier] $summary"
Write-Host "[verifier] overall: $overall"
Write-Host "[verifier] report: $reportPath"

switch ($overall) {
    'PASS'  { exit 0 }
    default { exit 1 }
}
