<#
.SYNOPSIS
  Static linter for code-reviewer's review.md. Catches mechanical mistakes
  (malformed Findings table, missing Verdict, bad severity, bad File:Line
  format) before the human reviewer reads the file.

.PARAMETER ReviewPath
  Path to review.md.

.NOTES
  Exit codes:
    0  no issues
    1  soft-fail (issues listed; reviewer agent should fix and re-run)
    3  invalid args / file not found
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ReviewPath
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $ReviewPath)) {
    Write-Error "[lint-review] file not found: $ReviewPath"
    exit 3
}

$text = Get-Content $ReviewPath -Raw
$issues = @()

# ---- Required sections ---------------------------------------------------
if ($text -notmatch '(?im)^\s*##\s+Findings\b')  { $issues += 'Missing required section: ## Findings' }
if ($text -notmatch '(?im)^\s*##\s+Verdict\b')   { $issues += 'Missing required section: ## Verdict' }
if ($text -notmatch '(?im)^\s*##\s+Summary\b')   { $issues += 'Missing required section: ## Summary' }

# ---- Findings table ------------------------------------------------------
$findingsMatch = [regex]::Match(
    $text,
    '(?ms)^##\s+Findings\s*\r?\n(?<body>.*?)(?=^##\s|\Z)'
)
$validSeverities = @('blocking','warning','info')
$rowCount = 0

if ($findingsMatch.Success) {
    $body = $findingsMatch.Groups['body'].Value
    $lines = $body -split "`r?`n" | Where-Object { $_ -match '^\s*\|' }

    if ($lines.Count -lt 2) {
        $issues += 'Findings section has no markdown table (need header + divider rows).'
    } else {
        $header = $lines[0]
        $divider = $lines[1]
        $expectedCols = @('#','Severity','File:Line','Rationale','Suggested fix')
        foreach ($col in $expectedCols) {
            if ($header -notmatch [regex]::Escape($col)) {
                $issues += "Findings header missing column: '$col'"
            }
        }
        if ($divider -notmatch '^\s*\|[\s\-\|:]+\|\s*$') {
            $issues += "Findings divider row malformed (line: '$divider')"
        }

        # Body rows
        $dataRows = $lines | Select-Object -Skip 2
        $rowIdx = 0
        foreach ($row in $dataRows) {
            $rowIdx++
            $cells = ($row -split '\|') | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
            if ($cells.Count -lt 5) {
                $issues += "Finding row #${rowIdx}: expected 5 cells, got $($cells.Count) — '$row'"
                continue
            }
            $rowCount++
            $sev = $cells[1].ToLowerInvariant()
            $loc = $cells[2]
            if ($validSeverities -notcontains $sev) {
                $issues += "Finding row #${rowIdx}: invalid severity '$($cells[1])' (allowed: $($validSeverities -join ', '))"
            }
            # File:Line format — accept src/... or tests/... with :<line> suffix.
            # Also accept multi-line refs like src/foo.cs:42-50.
            if ($loc -notmatch '^(src|tests|docs|scripts|\.github|\.claude)/.+:\d+(-\d+)?$' -and $loc -ne 'n/a') {
                $issues += "Finding row #${rowIdx}: File:Line '$loc' does not match expected pattern (e.g. 'src/.../X.cs:42' or 'n/a')"
            }
        }
    }
} else {
    # already reported above
}

# ---- Verdict line --------------------------------------------------------
$verdictMatch = [regex]::Match($text, '(?ims)^##\s+Verdict\s*\r?\n(?<body>.*?)(?=^##\s|\Z)')
if ($verdictMatch.Success) {
    $vbody = $verdictMatch.Groups['body'].Value
    $hasVerdictKeyword = ($vbody -match '(?i)\b(APPROVE( WITH NITS)?|REQUEST[_ ]CHANGES|ABORT)\b')
    if (-not $hasVerdictKeyword) {
        $issues += 'Verdict section must contain one of: APPROVE | APPROVE WITH NITS | REQUEST_CHANGES | REQUEST CHANGES | ABORT'
    }
}

# ---- Output --------------------------------------------------------------
if ($issues.Count -eq 0) {
    Write-Host "[lint-review] OK — review.md passed all checks ($rowCount finding(s))."
    exit 0
}

Write-Host "[lint-review] $($issues.Count) issue(s) in $ReviewPath" -ForegroundColor Yellow
foreach ($i in $issues) { Write-Host "  - $i" }
exit 1
