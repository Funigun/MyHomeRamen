#!/usr/bin/env pwsh
# agents-check-drift.ps1
# Verifies that the body (post-frontmatter) of each agent definition is identical
# between the Copilot mirror (.github/agents/<name>.agent.md) and the Claude mirror
# (.claude/agents/<name>.md). Frontmatters are intentionally different per platform.
# Exit 0 = all in sync, Exit 1 = drift detected.

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$agents = @('planner','implementer','verifier','code-reviewer','pr-commit','orchestrator')

function Get-AgentBody {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return $null }
    $raw = Get-Content $Path -Raw
    $i1 = $raw.IndexOf('---')
    if ($i1 -lt 0) { return $raw }
    $i2 = $raw.IndexOf('---', $i1 + 3)
    if ($i2 -lt 0) { return $raw }
    return $raw.Substring($i2 + 3).TrimStart("`r","`n")
}

function Get-Sha256 {
    param([string]$Text)
    if ($null -eq $Text) { return '<missing>' }
    $sha = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
    return [BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-','').ToLower()
}

$drift = $false
foreach ($a in $agents) {
    $copilotPath = Join-Path $repoRoot ".github/agents/$a.agent.md"
    $claudePath  = Join-Path $repoRoot ".claude/agents/$a.md"
    $copilotBody = Get-AgentBody $copilotPath
    $claudeBody  = Get-AgentBody $claudePath
    $h1 = Get-Sha256 $copilotBody
    $h2 = Get-Sha256 $claudeBody
    if ($h1 -eq $h2 -and $h1 -ne '<missing>') {
        Write-Host "[OK]    $a  ($($h1.Substring(0,12)))"
    } else {
        $drift = $true
        Write-Host "[DRIFT] $a"
        Write-Host "        copilot: $h1"
        Write-Host "        claude : $h2"
    }
}

if ($drift) {
    Write-Host ""
    Write-Host "DRIFT detected. Sync bodies between .github/agents/<name>.agent.md and .claude/agents/<name>.md (frontmatter is platform-specific and may differ)."
    exit 1
}
Write-Host ""
Write-Host "All agents in sync."
exit 0
