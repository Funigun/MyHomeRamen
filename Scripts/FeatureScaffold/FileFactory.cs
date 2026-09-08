#:include FeatureDetails.cs

public static class FileFactory
{
    private const string _withResponseEndpointTemplate =
"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{FEATURE};

public sealed record {FEATURE}Request();

{ENDPOINT_RESPONSE}

public sealed class {FEATURE}Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandard{ENDPOINT_TYPE}("{ROUTE}", HandleAsync)
            .WithName("{FEATURE}Endpoint")
            .WithTags("/*ToDo: complete tags*/")
            .WithDescription("ToDo: complete description")
            .RequireAuthorization(); // .AllowAnonymous();
    }

    //ToDo: Update IResult to proper response
    private static async Task<IResult> HandleAsync(
        [FromServices] {HANDLER_TYPE} handler,
        CancellationToken cancellationToken)
    {
        /*Todo: add request parameters and pass to {CQRS_LOWERCASE} if needed*/
        {FEATURE}{CQRS} {CQRS_LOWERCASE} = new();
        {HANDLER_RESPONSE_VARIABLE}await handler.Handle({CQRS_LOWERCASE}, cancellationToken);

        // ToDo: return proper response e.g. Ok(response), CreatedAtRoute, etc
        return TypedResults.Ok();
    }
}
""";

    private const string _cqrsTemplate =
"""
using FluentValidation;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.{Module}.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{FEATURE};

public sealed record {FEATURE}{CQRS}() : I{CQRS}{CQRS_RESPONSE_TYPE};

public sealed record {FEATURE}Dto(/*ToDo: complete DTO shape*/);

/*public sealed record {FEATURE}{CQRS}Options() : DbQueryOptions<TEntity, {FEATURE}Response>
(
    //ToDo: complete query options including setting proper generic types for TEntity and projection
);*/

public sealed class {FEATURE}AuthorizationPolicy() : IAuthorizationPolicy<{FEATURE}{CQRS}>
{
    public async Task<bool> Authorize({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement authorization logic
        return false;
    }
}

public sealed class {FEATURE}ValidationPolicy : AbstractValidator<{FEATURE}{CQRS}>
{
    public {FEATURE}ValidationPolicy(I{Module}DbContext dbContext)
    {
        //ToDo: implement validation rules
    }
}

public sealed class {FEATURE}Handler(I{Module}DbContext dbContext) : {HANDLER_TYPE}
{
    public async Task<{HANDLER_TASK_RETURN_TYPE}> Handle({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
        return {HANDLER_RETURN_TYPE};
    }
}
""";

    private const string _integrationTestTemplate =
"""
using System.Net;
using Bogus;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.{Module}Api.IntegrationTests.Common;
using MyHomeRamen.{Module}Api.IntegrationTests.Common.Data;

namespace MyHomeRamen.{Module}Api.IntegrationTests.{Aggregate};

public sealed class {FEATURE}Tests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        //ToDo: implement necessary seeding or setup logic
    }

    public async ValueTask DisposeAsync()
    {
        //ToDo: remove seeded data or cleanup logic
    }

    // ToDo: Happy path test

    // ToDo: Unauthorized test

    // ToDo: Forbidden test

    // ToDo: Validation test
}
""";

    public static string CreateEndpoint(FeatureDetails featureDetails) => ReplacePlaceholders(_withResponseEndpointTemplate, featureDetails);
    

    public static string CreateCqrs(FeatureDetails featureDetails) => ReplacePlaceholders(_cqrsTemplate, featureDetails);


    public static string CreateIntegrationTest(FeatureDetails featureDetails) => ReplacePlaceholders(_integrationTestTemplate, featureDetails);


    private static string ReplacePlaceholders(string template, FeatureDetails featureDetails)
    {
        string responseTypeContent = featureDetails.RequireResponse ? "<{FEATURE}Response>" : string.Empty;
        string handlerType = featureDetails.RequireResponse
                           ? $"IRequestHandler<{featureDetails.Name}{featureDetails.Command.Type}, {featureDetails.Name}Response>" 
                           : $"IRequestHandler<{featureDetails.Name}{featureDetails.Command.Type}, Unit>";

        string handlerReturnType = featureDetails.RequireResponse ? "new {FEATURE}Response()" : "Unit.Value";
        string handlerTaskReturnType = featureDetails.RequireResponse ? "{FEATURE}Response" : "Unit";


        string endpointResponse = featureDetails.RequireResponse
                                ? $"public sealed record {featureDetails.Name}Response();"
                                : string.Empty;

        string HandlerResponseVariable = featureDetails.RequireResponse ? "{FEATURE}Response response = " : string.Empty;

        string endpointType = featureDetails.ScaffoldIntegrationTest 
                            ? "" 
                            : featureDetails.Endpoint.Type switch
                              {
                                  "Get" => "Get<{FEATURE}Response>",
                                  "Post" => "Post<{FEATURE}Response>",
                                  "Put" => "Put",
                                  "Delete" => "Delete",
                                  _ => throw new ArgumentException($"Unsupported endpoint type: {featureDetails.Endpoint.Type}")
                              };

        return template.Replace("{CQRS_RESPONSE_TYPE}", responseTypeContent, StringComparison.Ordinal)
                       .Replace("{HANDLER_TYPE}", handlerType, StringComparison.Ordinal)
                       .Replace("{HANDLER_RESPONSE_VARIABLE}", HandlerResponseVariable, StringComparison.Ordinal)
                       .Replace("{HANDLER_TASK_RETURN_TYPE}", handlerTaskReturnType, StringComparison.Ordinal)
                       .Replace("{HANDLER_RETURN_TYPE}", handlerReturnType, StringComparison.Ordinal)
                       .Replace("{ENDPOINT_RESPONSE}", endpointResponse, StringComparison.Ordinal)
                       .Replace("{ENDPOINT_TYPE}", endpointType, StringComparison.Ordinal)
                       .Replace("{ROUTE}", featureDetails.Endpoint.Route ?? "\"/TODO\"", StringComparison.Ordinal)
                       .Replace("{FEATURE}", featureDetails.Name, StringComparison.Ordinal)
                       .Replace("{Aggregate}", featureDetails.Aggregate, StringComparison.Ordinal)
                       .Replace("{Module}", featureDetails.Module, StringComparison.Ordinal)
                       .Replace("{CQRS}", featureDetails.Command.Type, StringComparison.Ordinal)
                       .Replace("{CQRS_LOWERCASE}", featureDetails.Command.Type.ToLower(), StringComparison.Ordinal);
    }
}
