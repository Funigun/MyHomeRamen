#:include FileType.cs
#:include FileDetails.cs

public static class FileScaffoldFactory
{
    // Placeholders used in all templates
    private const string PlNs          = "{NS}";
    private const string PlContractsNs = "{CONTRACTS_NS}";
    private const string PlTypeName    = "{TYPE_NAME}";
    private const string PlFeat        = "{FEAT}";
    private const string PlEntity      = "{ENTITY}";

    private static string Apply(string template, FeatureDetails p, string? ns = null, string? contractsNs = null)
        => template
            .Replace(PlNs,          ns          ?? string.Empty)
            .Replace(PlContractsNs, contractsNs ?? string.Empty)
            .Replace(PlTypeName,    p.TypeName)
            .Replace(PlFeat,        p.Subfolder)
            .Replace(PlEntity,      p.Entity);

    public static string CreateFileScaffold(FileDetails fileDetails)
    {
        return fileDetails.Type switch
        {
            FileType.Request => BuildRequest(fileDetails.Feature),
            FileType.Response => BuildResponse(fileDetails.Feature),
            FileType.Command => BuildCommand(fileDetails.Feature),
            FileType.CommandVoid => BuildCommandVoid(fileDetails.Feature),
            FileType.Query => BuildQuery(fileDetails.Feature),
            FileType.CommandHandler => BuildCommandHandler(fileDetails.Feature),
            FileType.CommandVoidHandler => BuildCommandVoidHandler(fileDetails.Feature),
            FileType.QueryHandler  => BuildQueryHandler(fileDetails.Feature),
            FileType.Validator => BuildValidator(fileDetails.Feature),
            FileType.GetEndpoint => BuildEndpointGet(fileDetails.Feature),
            FileType.PostEndpoint => BuildEndpointPost(fileDetails.Feature),
            FileType.PutEndpoint => BuildEndpointPut(fileDetails.Feature),
            FileType.DeleteEndpoint => BuildEndpointDelete(fileDetails.Feature),
            FileType.UnitTest => BuildUnitTest(fileDetails.Feature),
            FileType.IntegrationTest => BuildIntegrationTest(fileDetails.Feature),
            _ => string.Empty
        };
    }

    // MyHomeRamen.Common.Contracts.{Module}.{Entity}.Requests
    private static string BuildRequest(FeatureDetails p)
    {
        const string template =
"""
namespace {NS};

public sealed record {TYPE_NAME}(/* TODO: complete request properties */);
""";
        string ns = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}.Requests";
        return Apply(template, p, ns);
    }

    private static string BuildResponse(FeatureDetails p)
    {
        const string template =
"""
namespace {NS};

public sealed record {TYPE_NAME}(/* TODO: complete response properties */);
""";
        string ns = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}.Responses";
        return Apply(template, p, ns);
    }

    // Subfolder = Feature name (e.g. ClearBasket)
    private static string BuildCommand(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Requests;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed record {TYPE_NAME}(/* TODO: complete command properties */) : ICommand<{FEAT}Response>;
""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildCommandVoid(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace {NS};

public sealed record {TYPE_NAME}(/* TODO: complete command properties */) : ICommand;
""";
        string ns = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        return Apply(template, p, ns);
    }

    private static string BuildQuery(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed record {TYPE_NAME}(/* TODO: complete query properties */) : IQuery<{FEAT}Response>;
""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildCommandHandler(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed class {TYPE_NAME}(/* TODO: inject dependencies */) : ICommandHandler<{FEAT}Command, {FEAT}Response>
{
    public async Task<{FEAT}Response> Handle({FEAT}Command command, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}

""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildCommandVoidHandler(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace {NS};

public sealed class {TYPE_NAME}(/* TODO: inject dependencies */) : ICommandHandler<{FEAT}Command>
{
    public async Task Handle({FEAT}Command command, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}

""";
        string ns = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        return Apply(template, p, ns);
    }

    private static string BuildQueryHandler(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed class {TYPE_NAME}(/* TODO: inject dependencies */) : IQueryHandler<{FEAT}Query, {FEAT}Response>
{
    public async Task<{FEAT}Response> Handle({FEAT}Query query, CancellationToken cancellationToken)
    {
        // TODO: implement -- see plan
        throw new NotImplementedException();
    }
}

""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildValidator(FeatureDetails p)
    {
        const string template =
"""
using FluentValidation;

namespace {NS};

public sealed class {TYPE_NAME} : AbstractValidator<{FEAT}Command>
{
    // TODO: inject IDbContext if async validation rules are needed
    public {TYPE_NAME}()
    {
        // TODO: add validation rules per plan
    }
}

""";
        string ns = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        return Apply(template, p, ns);
    }

    private static string BuildEndpointGet(FeatureDetails p)
    {
        const string template =
"""
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed class {TYPE_NAME} : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardGet<{FEAT}Response>("api/TODO", HandleAsync)
            .WithName("{TYPE_NAME}")
            .WithTags("{ENTITY}");
    }

    private static async Task<Results<Ok<{FEAT}Response>, NotFound>> HandleAsync(
        // TODO: add route/query params
        [FromServices] IQueryHandler<{FEAT}Query, {FEAT}Response> handler,
        CancellationToken cancellationToken)
    {
        {FEAT}Query query = new(/* TODO */);
        {FEAT}Response? response = await handler.Handle(query, cancellationToken);

        return response is null ? TypedResults.NotFound() : TypedResults.Ok(response);
    }
}

""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildEndpointPost(FeatureDetails p)
    {
        const string template =
"""
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Requests;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed class {TYPE_NAME} : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardPost<{FEAT}Response>("api/TODO", HandleAsync)
            .WithName("{TYPE_NAME}")
            .WithTags("{ENTITY}");
    }

    private static async Task<Results<Created<{FEAT}Response>, BadRequest>> HandleAsync(
        [FromBody] {FEAT}Request request,
        [FromServices] ICommandHandler<{FEAT}Command, {FEAT}Response> handler,
        CancellationToken cancellationToken)
    {
        {FEAT}Command command = new(request);
        {FEAT}Response response = await handler.Handle(command, cancellationToken);

        // TODO: update Created location URL
        return TypedResults.Created($"/api/TODO/{response}", response);
    }
}

""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildEndpointPut(FeatureDetails p)
    {
        const string template =
"""
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using {CONTRACTS_NS}.Requests;
using {CONTRACTS_NS}.Responses;

namespace {NS};

public sealed class {TYPE_NAME} : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route (typically includes {id})
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardPut<{FEAT}Response>("api/TODO/{id}", HandleAsync)
            .WithName("{TYPE_NAME}")
            .WithTags("{ENTITY}");
    }

    private static async Task<Results<Ok<{FEAT}Response>, NotFound, BadRequest>> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] {FEAT}Request request,
        [FromServices] ICommandHandler<{FEAT}Command, {FEAT}Response> handler,
        CancellationToken cancellationToken)
    {
        {FEAT}Command command = new(new(id), request);
        {FEAT}Response response = await handler.Handle(command, cancellationToken);

        return TypedResults.Ok(response);
    }
}

""";
        string ns          = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        string contractsNs = $"MyHomeRamen.Common.Contracts.{p.Module}.{p.Entity}";
        return Apply(template, p, ns, contractsNs);
    }

    private static string BuildEndpointDelete(FeatureDetails p)
    {
        const string template =
"""
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace {NS};

public sealed class {TYPE_NAME} : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            // TODO: set correct route (typically includes {id})
            // TODO: set auth policy (.AllowAnonymous() or .RequireAuthorization(...))
            .MapStandardDelete("api/TODO/{id}", HandleAsync)
            .WithName("{TYPE_NAME}")
            .WithTags("{ENTITY}");
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> HandleAsync(
        // TODO: add route params
        [FromServices] ICommandHandler<{FEAT}Command> handler,
        CancellationToken cancellationToken)
    {
        {FEAT}Command command = new(/* TODO */);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}

""";
        string ns = $"MyHomeRamen.Api.{p.Module}.Features.{p.Entity}.{p.Subfolder}";
        return Apply(template, p, ns);
    }

    private static string BuildUnitTest(FeatureDetails p)
    {
        const string template =
"""
namespace {NS};

public sealed class {TYPE_NAME}
{
    // TODO: implement unit test cases per plan
}

""";
        string ns = $"MyHomeRamen.UnitTests.{p.Module}Module.{p.Entity}";
        return Apply(template, p, ns);
    }

    private static string BuildIntegrationTest(FeatureDetails p)
    {
        const string template =
"""
using MyHomeRamen.IntegrationTests.Common;

namespace {NS};

public sealed class {TYPE_NAME}(WebApiFactory apiFactory)
{
    // TODO: implement integration test cases per plan
}

""";
        string ns = $"MyHomeRamen.IntegrationTests.{p.Module}Module.{p.Entity}";
        return Apply(template, p, ns);
    }
}
