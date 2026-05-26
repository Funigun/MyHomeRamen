#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Scaffold deterministic stubs for a feature slice from the #2 Files table in a plan file.

.DESCRIPTION
  Reads the markdown table in #2 ("Files to create / modify") and creates skeleton
  files for every row where Action = `create`. Rows with Action = `modify` are logged
  but never touched. Existing files are never overwritten (idempotent).

  ## #2 table format

  | Path | Action | Type | Notes |
  |------|--------|------|-------|
  | `MyHomeRamen.Api\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemCommand.cs` | create | command-void | |
  | `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | modify | | Add RemoveItem |

  ### Type keywords

  Contracts (MyHomeRamen.Common.Contracts\{Module}\{Entity}\Requests|Responses\{TypeName}.cs):
    request           -> sealed record {TypeName}(/* TODO */)
    response          -> sealed record {TypeName}(/* TODO */)

  API slice ({Project}\{Module}\Features\{Entity}\{Feature}\{TypeName}.cs):
    command           -> ICommand<{Feature}Response>
    command-void      -> ICommand  (no response -- DELETE / void operations)
    query             -> IQuery<{Feature}Response>
    command-handler   -> ICommandHandler<{Feature}Command, {Feature}Response>
    command-void-handler -> ICommandHandler<{Feature}Command>
    query-handler     -> IQueryHandler<{Feature}Query, {Feature}Response>
    validator         -> AbstractValidator<{Feature}Command|Query>
    endpoint-get      -> MapStandardGet,    IQueryHandler,   returns Ok / NotFound
    endpoint-post     -> MapStandardPost,   ICommandHandler, returns Created
    endpoint-put      -> MapStandardPut,    ICommandHandler, returns Ok
    endpoint-delete   -> MapStandardDelete, ICommandHandler, returns NoContent

  Tests:
    unit-test         -> empty class with TODO comment
    integration-test  -> class(WebApiFactory) with TODO comment

  Rows with empty or unrecognised Type and Action = `create` are logged as unsupported
  (e.g. persistence extension files -- create those by hand).

.PARAMETER PlanPath
  Path to the plan markdown file.

.EXAMPLE
  pwsh .github/scripts/slice-scaffold.ps1 -PlanPath .github/plans/shopping-cart/delete-item-plan-backend.md
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

# -- Parse #2 markdown table --
$lines = Get-Content $PlanPath
$inSec2 = $false
$tableRows = New-Object System.Collections.Generic.List[string]
foreach ($ln in $lines) {
    if ($ln -match '^##\s+2\.') { $inSec2 = $true; continue }
    if ($ln -match '^##\s+\d+\.' -and $inSec2) { break }
    if ($inSec2 -and $ln -match '^\s*\|') { $tableRows.Add($ln) | Out-Null }
}

if ($tableRows.Count -lt 3) {
    Write-Host "[slice-scaffold] #2 table not found or empty in $PlanPath"
    exit 3
}

# Skip header (row 0) and divider (row 1)
$entries = @()
for ($i = 2; $i -lt $tableRows.Count; $i++) {
    $cols = ($tableRows[$i] -split '\|') | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
    if ($cols.Count -lt 2) { continue }
    $entries += [pscustomobject]@{
        Path   = ($cols[0] -replace '^`|`$', '').Trim()
        Action = $cols[1].ToLowerInvariant().Trim()
        Type   = if ($cols.Count -ge 3) { ($cols[2] -replace '^`|`$', '').ToLowerInvariant().Trim() } else { '' }
    }
}

# -- Helpers --
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

# -- Path parsers --

# {Project}\{Module}\Features\{Entity}\{Feature}\{TypeName}.cs
function Parse-ApiPath([string]$path) {
    $p = $path -replace '\\', '/'
    if ($p -match '^(MyHomeRamen\.[^/]+)/([^/]+)/Features/([^/]+)/([^/]+)/([^/]+)\.cs$') {
        return @{ Project = $Matches[1]; Module = $Matches[2]; Entity = $Matches[3]; Feature = $Matches[4]; TypeName = $Matches[5] }
    }
    return $null
}

# MyHomeRamen.Common.Contracts\{Module}\{Entity}\Requests|Responses\{TypeName}.cs
function Parse-ContractsPath([string]$path) {
    $p = $path -replace '\\', '/'
    if ($p -match '^MyHomeRamen\.Common\.Contracts/([^/]+)/([^/]+)/([^/]+)/([^/]+)\.cs$') {
        return @{ Module = $Matches[1]; Entity = $Matches[2]; SubFolder = $Matches[3]; TypeName = $Matches[4] }
    }
    return $null
}

# MyHomeRamen.{Tests}\{Module}Module\{Entity}\{TypeName}.cs
function Parse-TestPath([string]$path) {
    $p = $path -replace '\\', '/'
    if ($p -match '^(MyHomeRamen\.[^/]+)/([^/]+)Module/([^/]+)/([^/]+)\.cs$') {
        return @{ Project = $Matches[1]; Module = $Matches[2]; Entity = $Matches[3]; TypeName = $Matches[4] }
    }
    return $null
}

# -- Template builders --

function Build-Request([hashtable]$p) {
    $ns = "MyHomeRamen.Common.Contracts.$($p.Module).$($p.Entity).Requests"
@"
namespace $ns;

public sealed record $($p.TypeName)(/* TODO: complete request properties */);
"@
}

function Build-Response([hashtable]$p) {
    $ns = "MyHomeRamen.Common.Contracts.$($p.Module).$($p.Entity).Responses"
@"
namespace $ns;

public sealed record $($p.TypeName)(/* TODO: complete response properties */);
"@
}

function Build-Command([hashtable]$p) {
    $ns     = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat   = $p.Feature
    $module = $p.Module
    $entity = $p.Entity
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Requests;
using ${contractsNs}.Responses;

namespace $ns;

public sealed record $($p.TypeName)(/* TODO: complete command properties */) : ICommand<${feat}Response>;
"@
}

function Build-CommandVoid([hashtable]$p) {
    $ns = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace $ns;

public sealed record $($p.TypeName)(/* TODO: complete command properties */) : ICommand;
"@
}

function Build-Query([hashtable]$p) {
    $ns     = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat   = $p.Feature
    $module = $p.Module
    $entity = $p.Entity
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Responses;

namespace $ns;

public sealed record $($p.TypeName)(/* TODO: complete query properties */) : IQuery<${feat}Response>;
"@
}

function Build-CommandHandler([hashtable]$p) {
    $ns      = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat    = $p.Feature
    $module  = $p.Module
    $entity  = $p.Entity
    $cmdName = "${feat}Command"
    $respName = "${feat}Response"
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Responses;

namespace $ns;

public sealed class $($p.TypeName)(/* TODO: inject dependencies */) : ICommandHandler<${cmdName}, ${respName}>
{
    public async Task<${respName}> Handle(${cmdName} command, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}
"@
}

function Build-CommandVoidHandler([hashtable]$p) {
    $ns      = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat    = $p.Feature
    $cmdName = "${feat}Command"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace $ns;

public sealed class $($p.TypeName)(/* TODO: inject dependencies */) : ICommandHandler<${cmdName}>
{
    public async Task Handle(${cmdName} command, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}
"@
}

function Build-QueryHandler([hashtable]$p) {
    $ns       = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat     = $p.Feature
    $module   = $p.Module
    $entity   = $p.Entity
    $qryName  = "${feat}Query"
    $respName = "${feat}Response"
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Responses;

namespace $ns;

public sealed class $($p.TypeName)(/* TODO: inject dependencies */) : IQueryHandler<${qryName}, ${respName}>
{
    public async Task<${respName}> Handle(${qryName} query, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}
"@
}

function Build-Validator([hashtable]$p) {
    $ns     = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat   = $p.Feature
    # Infer whether this validates a command or query from the feature name  both are safe to leave as TODO
    $target = "${feat}Command"
@"
using FluentValidation;

namespace $ns;

public sealed class $($p.TypeName) : AbstractValidator<${target}>
{
    // TODO: inject IDbContext if async validation rules are needed
    public $($p.TypeName)()
    {
        // TODO: add validation rules per plan
    }
}
"@
}

function Build-EndpointGet([hashtable]$p) {
    $ns      = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat    = $p.Feature
    $tn      = $p.TypeName
    $module  = $p.Module
    $entity  = $p.Entity
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Responses;

namespace $ns;

public sealed class $tn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardGet<${feat}Response>("api/TODO", HandleAsync)
            .WithName("$tn")
            .WithTags("$entity");
    }

    private static async Task<Results<Ok<${feat}Response>, NotFound>> HandleAsync(
        // TODO: add route/query params ([FromRoute] Guid id, [AsParameters] ..., etc.)
        [FromServices] IQueryHandler<${feat}Query, ${feat}Response> handler,
        CancellationToken cancellationToken)
    {
        ${feat}Query query = new(/* TODO */);
        ${feat}Response? response = await handler.Handle(query, cancellationToken);

        return response is null ? TypedResults.NotFound() : TypedResults.Ok(response);
    }
}
"@
}

function Build-EndpointPost([hashtable]$p) {
    $ns      = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat    = $p.Feature
    $tn      = $p.TypeName
    $module  = $p.Module
    $entity  = $p.Entity
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Requests;
using ${contractsNs}.Responses;

namespace $ns;

public sealed class $tn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardPost<${feat}Response>("api/TODO", HandleAsync)
            .WithName("$tn")
            .WithTags("$entity");
    }

    private static async Task<Results<Created<${feat}Response>, BadRequest>> HandleAsync(
        [FromBody] ${feat}Request request,
        [FromServices] ICommandHandler<${feat}Command, ${feat}Response> handler,
        CancellationToken cancellationToken)
    {
        ${feat}Command command = new(request);
        ${feat}Response response = await handler.Handle(command, cancellationToken);

        // TODO: update Created location URL
        return TypedResults.Created(`$"/api/TODO/{response}", response);
    }
}
"@
}

function Build-EndpointPut([hashtable]$p) {
    $ns      = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat    = $p.Feature
    $tn      = $p.TypeName
    $module  = $p.Module
    $entity  = $p.Entity
    $contractsNs = "MyHomeRamen.Common.Contracts.$module.$entity"
@"
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using ${contractsNs}.Requests;
using ${contractsNs}.Responses;

namespace $ns;

public sealed class $tn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route (typically includes {id})
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardPut<${feat}Response>("api/TODO/{id}", HandleAsync)
            .WithName("$tn")
            .WithTags("$entity");
    }

    private static async Task<Results<Ok<${feat}Response>, NotFound, BadRequest>> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] ${feat}Request request,
        [FromServices] ICommandHandler<${feat}Command, ${feat}Response> handler,
        CancellationToken cancellationToken)
    {
        ${feat}Command command = new(new(id), request);
        ${feat}Response response = await handler.Handle(command, cancellationToken);

        return TypedResults.Ok(response);
    }
}
"@
}

function Build-EndpointDelete([hashtable]$p) {
    $ns     = "$($p.Project).$($p.Module).Features.$($p.Entity).$($p.Feature)"
    $feat   = $p.Feature
    $tn     = $p.TypeName
    $entity = $p.Entity
@"
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace $ns;

public sealed class $tn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route (typically includes {id})
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardDelete("api/TODO/{id}", HandleAsync)
            .WithName("$tn")
            .WithTags("$entity");
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> HandleAsync(
        // TODO: add route params ([FromRoute] Guid id, etc.)
        [FromServices] ICommandHandler<${feat}Command> handler,
        CancellationToken cancellationToken)
    {
        ${feat}Command command = new(/* TODO */);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
"@
}

function Build-UnitTest([hashtable]$p) {
    $ns = "MyHomeRamen.UnitTests.$($p.Module)Module.$($p.Entity)"
@"
namespace $ns;

public sealed class $($p.TypeName)
{
    // TODO: implement unit test cases per plan
}
"@
}

function Build-IntegrationTest([hashtable]$p) {
    $ns = "MyHomeRamen.IntegrationTests.$($p.Module)Module.$($p.Entity)"
@"
using MyHomeRamen.IntegrationTests.Common;

namespace $ns;

public sealed class $($p.TypeName)(WebApiFactory apiFactory)
{
    // TODO: implement integration test cases per plan
}
"@
}

# -- Process entries --
foreach ($e in $entries) {
    if ($e.Action -eq 'modify') {
        $modified += $e.Path
        continue
    }

    if ($e.Action -ne 'create') {
        $unsupported += "$($e.Path) (unknown action: $($e.Action))"
        continue
    }

    $pNorm = $e.Path -replace '/', '\'

    $content = switch ($e.Type) {

        # -- Contracts --
        'request' {
            $parts = Parse-ContractsPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse contracts path)"; continue }
            Build-Request $parts
        }
        'response' {
            $parts = Parse-ContractsPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse contracts path)"; continue }
            Build-Response $parts
        }

        # -- API slice --
        'command' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-Command $parts
        }
        'command-void' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-CommandVoid $parts
        }
        'query' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-Query $parts
        }
        'command-handler' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-CommandHandler $parts
        }
        'command-void-handler' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-CommandVoidHandler $parts
        }
        'query-handler' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-QueryHandler $parts
        }
        'validator' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-Validator $parts
        }
        'endpoint-get' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-EndpointGet $parts
        }
        'endpoint-post' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-EndpointPost $parts
        }
        'endpoint-put' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-EndpointPut $parts
        }
        'endpoint-delete' {
            $parts = Parse-ApiPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse API path)"; continue }
            Build-EndpointDelete $parts
        }

        # -- Tests --
        'unit-test' {
            $parts = Parse-TestPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse test path)"; continue }
            Build-UnitTest $parts
        }
        'integration-test' {
            $parts = Parse-TestPath $pNorm
            if (-not $parts) { $unsupported += "$($e.Path) (could not parse test path)"; continue }
            Build-IntegrationTest $parts
        }

        default {
            $unsupported += "$($e.Path) (unknown type: '$($e.Type)')"
            $null
        }
    }

    if ($null -ne $content) {
        Ensure-File $pNorm $content
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
