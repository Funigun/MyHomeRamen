#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Scaffold deterministic stubs for a feature slice based on plan.approved.md §3.

.DESCRIPTION
  Reads the markdown table in §3 ("Files to create / modify") of an approved
  plan and creates skeleton files for entries marked `create`.

  ## §3 table format

  | File | Action | Type | Options | Rationale |
  |------|--------|------|---------|-----------|
  | `MyHomeRamen.Api\Orders\Features\Orders\CreateOrder\CreateOrderEndpoint.cs` | create | endpoint | verb=POST route=orders auth=RestaurantManagerPolicy | ... |

  ### Supported `Type` values

  Slice types (all live under MyHomeRamen.Api or MyHomeRamen.Identity.Api):
    endpoint              → {Op}{Entity}Endpoint.cs
    handler               → {Op}{Entity}Handler.cs
    request               → Models\{Op}{Entity}Request.cs
    response              → Models\{Op}{Entity}Response.cs
    mappings              → Models\Mappings.cs
    validator             → Policies\{Op}{Entity}Validator.cs        (optional)
    authorization-policy  → Policies\{Op}{Entity}AuthorizationPolicy.cs  (optional)

  Event types:
    domain-event          → MyHomeRamen.Domain\{Module}\Events\{Name}Event.cs
    integration-event     → MyHomeRamen.Common.Contracts\Messaging\{Name}IntegrationEvent.cs

  ### Supported `Options` keys
    verb   — HTTP verb: GET | POST | PUT | DELETE  (required for endpoint)
    route  — route template, e.g. orders or orders/{id}  (required for endpoint)
    auth   — constant name from AuthorizationDependencyInjection, e.g. RestaurantManagerPolicy
    group  — endpoint group name (defaults to the Module segment of the path)

  ### Path-derived naming
  For API slice files the script derives Module, Entity, Operation, TypeName from the
  path:
    MyHomeRamen.Api\{Module}\Features\{Entity}\{Operation}\{TypeName}.cs
  The same derivation applies to Identity.Api paths.

  Idempotent: existing files are never overwritten.
  `modify` rows are logged but not touched by the scaffold.
  Unknown types are logged as unsupported.

.PARAMETER PlanPath
  Path to plan.approved.md.

.EXAMPLE
  pwsh .github/scripts/slice-scaffold.ps1 -PlanPath .agent-run/abc/plan.approved.md
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $PlanPath
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '../..')

if (-not (Test-Path $PlanPath)) {
    Write-Host "[slice-scaffold] plan not found: $PlanPath"
    exit 3
}

# ── Parse §3 markdown table ──────────────────────────────────────────
$lines = Get-Content $PlanPath
$inSec3 = $false
$tableRows = New-Object System.Collections.Generic.List[string]
foreach ($ln in $lines) {
    if ($ln -match '^##\s+3\.') { $inSec3 = $true; continue }
    if ($ln -match '^##\s+\d+\.' -and $inSec3) { break }
    if ($inSec3 -and $ln -match '^\s*\|') { $tableRows.Add($ln) | Out-Null }
}

if ($tableRows.Count -lt 3) {
    Write-Host "[slice-scaffold] §3 table not found or empty in $PlanPath"
    exit 3
}

# Skip header (row 0) and divider (row 1)
$entries = @()
for ($i = 2; $i -lt $tableRows.Count; $i++) {
    $cols = ($tableRows[$i] -split '\|') | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
    if ($cols.Count -lt 3) { continue }
    $entries += [pscustomobject]@{
        Path      = ($cols[0] -replace '^`|`$', '').Trim()
        Action    = $cols[1].ToLowerInvariant().Trim()
        Type      = if ($cols.Count -ge 3) { $cols[2].ToLowerInvariant().Trim() } else { '' }
        Options   = if ($cols.Count -ge 4) { $cols[3].Trim() } else { '' }
        Rationale = if ($cols.Count -ge 5) { $cols[4].Trim() } else { '' }
    }
}

# ── Option parser ─────────────────────────────────────────────────────
function Parse-Options([string]$optStr) {
    $opts = @{}
    foreach ($token in ($optStr -split '\s+')) {
        if ($token -match '^([^=]+)=(.+)$') {
            $opts[$Matches[1].ToLowerInvariant()] = $Matches[2]
        }
    }
    return $opts
}

# ── Helpers ───────────────────────────────────────────────────────────
$created = @(); $skipped = @(); $modified = @(); $unsupported = @()

function Ensure-File([string]$relative, [string]$content) {
    $abs = Join-Path $repoRoot $relative
    if (Test-Path $abs) {
        $script:skipped += $relative
        return
    }
    $dir = Split-Path $abs -Parent
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    [System.IO.File]::WriteAllText($abs, $content, [System.Text.UTF8Encoding]::new($false))
    $script:created += $relative
}

# ── Parse API path into components ───────────────────────────────────
# Handles both direct files and files one subfolder deep (Models\ / Policies\):
#   {Project}\{Module}\Features\{Entity}\{Operation}\{TypeName}.cs
#   {Project}\{Module}\Features\{Entity}\{Operation}\{SubFolder}\{TypeName}.cs
function Parse-ApiPath([string]$path) {
    $p = $path -replace '\\', '/'
    # With subfolder (Models / Policies)
    if ($p -match '^(MyHomeRamen\.[^/]+)/([^/]+)/Features/([^/]+)/([^/]+)/[^/]+/([^/]+)\.cs$') {
        return @{
            Project   = $Matches[1]
            Module    = $Matches[2]
            Entity    = $Matches[3]
            Operation = $Matches[4]
            TypeName  = $Matches[5]
        }
    }
    # Direct file (Endpoint / Handler)
    if ($p -match '^(MyHomeRamen\.[^/]+)/([^/]+)/Features/([^/]+)/([^/]+)/([^/]+)\.cs$') {
        return @{
            Project   = $Matches[1]
            Module    = $Matches[2]
            Entity    = $Matches[3]
            Operation = $Matches[4]
            TypeName  = $Matches[5]
        }
    }
    return $null
}

# ── Template builders ─────────────────────────────────────────────────

function Build-Endpoint([hashtable]$parts, [hashtable]$opts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation"
    $group     = if ($opts['group']) { $opts['group'] } else { $module }

@"
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace $ns;

public sealed class $typeName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        // TODO: register route per plan
    }

    private static async Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        // TODO: implement per plan
        throw new NotImplementedException();
    }
}
"@
}

function Build-Handler([hashtable]$parts, [hashtable]$opts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation"
    $modelsNs  = "$ns.Models"
    $verb      = if ($opts['verb']) { $opts['verb'].ToUpper() } else { 'GET' }
    $reqType   = "${operation}Request"
    $respType  = "${operation}Response"

    $responseGeneric = switch ($verb) {
        'DELETE' { 'IResult' }
        'POST'   { 'Guid' }
        default  { $respType }
    }

    $handleSig = switch ($verb) {
        'DELETE' { "[FromRoute] $reqType id" }
        default  { "$reqType request" }
    }

    $handleParam = switch ($verb) {
        'DELETE' { 'id' }
        default  { 'request' }
    }

@"
using MyHomeRamen.Api.Common.Endpoint.Models;
using $modelsNs;

namespace $ns;

public sealed class $typeName(/* TODO: inject db context */) : IRequestHandler<$reqType, $responseGeneric>
{
    public async Task<$responseGeneric> Handle($handleSig, CancellationToken cancellationToken)
    {
        // TODO: implement — see plan
        throw new NotImplementedException();
    }
}
"@
}

function Build-Request([hashtable]$parts, [hashtable]$opts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation.Models"
    $verb      = if ($opts['verb']) { $opts['verb'].ToUpper() } else { 'GET' }
    $respType  = "${operation}Response"

    # Route-bound single-ID requests (GET by id / DELETE)
    $isRouteId = ($verb -eq 'DELETE') -or ($verb -eq 'GET' -and $opts['route'] -match '\{id\}')
    # PUT carries body + route ID
    $isPut = ($verb -eq 'PUT')

    if ($isRouteId) {
@"
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace $ns;

public record struct $typeName : IRequestId<$typeName>, IRequest<$(if ($verb -eq 'DELETE') { 'IResult' } else { $respType })>
{
    public Guid Id { get; set; }
}
"@
    } elseif ($isPut) {
@"
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace $ns;

public sealed record $typeName(/* TODO: body properties */) : IRequest<$respType>
{
    [RouteParam]
    public Guid Id { get; init; }
}
"@
    } else {
@"
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace $ns;

public sealed record $typeName(/* TODO: properties */) : IRequest<$respType>;
"@
    }
}

function Build-Response([hashtable]$parts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation.Models"

@"
namespace $ns;

public sealed record $typeName(/* TODO: response properties */);
"@
}

function Build-Mappings([hashtable]$parts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $ns        = "$proj.$module.Features.$entity.$operation.Models"

@"
namespace $ns;

internal static class Mappings
{
    // TODO: add extension methods to map between domain objects and request/response models
}
"@
}

function Build-Validator([hashtable]$parts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation.Policies"
    $modelsNs  = "$proj.$module.Features.$entity.$operation.Models"
    $reqType   = "${operation}Request"

@"
using FluentValidation;
using $modelsNs;

namespace $ns;

public sealed class $typeName : AbstractValidator<$reqType>
{
    public $typeName(/* TODO: inject db context if async rules needed */)
    {
        // TODO: add validation rules per plan
    }
}
"@
}

function Build-AuthorizationPolicy([hashtable]$parts) {
    $proj      = $parts.Project
    $module    = $parts.Module
    $entity    = $parts.Entity
    $operation = $parts.Operation
    $typeName  = $parts.TypeName
    $ns        = "$proj.$module.Features.$entity.$operation"
    $modelsNs  = "$ns.Models"
    $respType  = "${operation}Response"

@"
using MyHomeRamen.Api.Common.Authorization;
using $modelsNs;

namespace $ns;

public sealed class $typeName(ICurrentUser currentUser) : IAuthorizationPolicy<$respType>
{
    public async Task<bool> IsAuthorized($respType response)
    {
        // TODO: implement authorization logic per plan.approved.md §5
        return await Task.FromResult(false);
    }
}
"@
}

function Build-DomainEvent([string]$path, [string]$typeName) {
    # Derive namespace from path: MyHomeRamen.Domain\{Module}\Events\{Name}Event.cs
    $p = $path -replace '\\', '/'
    if ($p -match '^(MyHomeRamen\.Domain)/([^/]+)/Events/') {
        $ns = "MyHomeRamen.Domain.$($Matches[2]).Events"
    } else {
        $ns = 'MyHomeRamen.Domain.Events'
    }

@"
using MyHomeRamen.Api.Common.Domain;

namespace $ns;

public sealed class $typeName(/* TODO: add relevant domain object */) : IDomainEvent
{
    // TODO: expose properties needed by event handlers
}
"@
}

function Build-IntegrationEvent([string]$typeName) {
@"
namespace MyHomeRamen.Common.Contracts.Messaging;

public record $typeName(
    Guid Id
    // TODO: add event properties
);
"@
}

# ── Process entries ───────────────────────────────────────────
foreach ($e in $entries) {
    if ($e.Action -eq 'modify') {
        $modified += $e.Path
        continue
    }

    if ($e.Action -ne 'create') {
        $unsupported += "$($e.Path) (unknown action: $($e.Action))"
        continue
    }

    $opts  = Parse-Options $e.Options
    $pNorm = $e.Path -replace '/', '\'

    switch ($e.Type) {

        # ── Slice files (endpoint / handler / models / policies) ──────
        { $_ -in @('endpoint','handler','request','response','mappings','validator','authorization-policy') } {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) {
                $unsupported += "$($e.Path) (could not parse API path for type '$($e.Type)')"
                continue
            }

            $content = switch ($e.Type) {
                'endpoint'             { Build-Endpoint            $parts $opts }
                'handler'              { Build-Handler             $parts $opts }
                'request'              { Build-Request             $parts $opts }
                'response'             { Build-Response            $parts }
                'mappings'             { Build-Mappings            $parts }
                'validator'            { Build-Validator           $parts }
                'authorization-policy' { Build-AuthorizationPolicy $parts }
            }
            Ensure-File $pNorm $content
            break
        }

        # ── Domain event ──────────────────────────────────────────────
        'domain-event' {
            $typeName = [System.IO.Path]::GetFileNameWithoutExtension($pNorm)
            Ensure-File $pNorm (Build-DomainEvent $pNorm $typeName)
            break
        }

        # ── Integration event ─────────────────────────────────────────
        'integration-event' {
            $typeName = [System.IO.Path]::GetFileNameWithoutExtension($pNorm)
            Ensure-File $pNorm (Build-IntegrationEvent $typeName)
            break
        }

        default {
            $unsupported += "$($e.Path) (unknown type: '$($e.Type)')"
        }
    }
}

Write-Host ""
Write-Host "[slice-scaffold] plan: $PlanPath"
Write-Host "[slice-scaffold] created:     $($created.Count)"
foreach ($x in $created)     { Write-Host "  + $x" }
Write-Host "[slice-scaffold] skipped:     $($skipped.Count) (already exist)"
foreach ($x in $skipped)     { Write-Host "  = $x" }
Write-Host "[slice-scaffold] modify rows: $($modified.Count) (hand-edit required)"
foreach ($x in $modified)    { Write-Host "  ~ $x" }
Write-Host "[slice-scaffold] unsupported: $($unsupported.Count)"
foreach ($x in $unsupported) { Write-Host "  ? $x" }
exit 0
