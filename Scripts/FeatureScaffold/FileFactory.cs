#:include FeatureDetails.cs

private const string _withResponseEndpointTemplate =
"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.{CQRS};
using MyHomeRamen.Features.Common.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyHomeRamen.Features.Menu.Features.{Aggregate}.{FEATURE};

{REQUEST_CONTRACT}

{RESPONSE_CONTRACT}

public sealed class {FEATURE}Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandard{ENDPOINT_TYPE}<{FEATURE}Response>({ROUTE}, HandleAsync)
            .WithName("{FEATURE}Endpoint")
            .WithTags("/*ToDo: complete tags*/")
            .WithDescription(/*ToDo: complete description*/)
            .RequireAuthorization(); // .AllowAnonymous();
    }

    private static async Task<Results<Ok<{FEATURE}Response>, BadRequest>> HandleAsync(
        [FromServices] I{CQRS}Handler<{FEATURE}{CQRS}, {FEATURE}Response> handler, 
        CancellationToken cancellationToken)
    {
        /*Todo: add request parameters and pass to {CQRS_LOWERCASE} if needed*/
        {FEATURE}{CQRS} {CQRS_LOWERCASE} = new();
        {FEATURE}Response response = await handler.Handle({CQRS_LOWERCASE}, cancellationToken);

        return TypedResults.Ok(response);
    }
}
""";

private const string _withoutResponseEndpointTemplate =
"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.{CQRS};
using MyHomeRamen.Features.Common.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyHomeRamen.Features.Menu.Features.{Aggregate}.{FEATURE};

{REQUEST_CONTRACT}

public sealed class {FEATURE}Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandard{ENDPOINT_TYPE}<{FEATURE}Response>({ROUTE}, HandleAsync)
            .WithName("{FEATURE}Endpoint")
            .WithTags("/*ToDo: complete tags*/")
            .WithDescription(/*ToDo: complete description*/)
            .RequireAuthorization(); // .AllowAnonymous();
    }

    private static async Task<Results<Ok, BadRequest>> HandleAsync(
        [FromServices] I{CQRS}Handler<{FEATURE}{CQRS}> handler,
        CancellationToken cancellationToken)
    {
        /*Todo: add request parameters and pass to {CQRS_LOWERCASE} if needed*/
        {FEATURE}{CQRS} {CQRS_LOWERCASE} = new();
        await handler.Handle({CQRS_LOWERCASE}, cancellationToken);

        return TypedResults.Ok();
    }
}
""";

private const string _cqrsHandlerWithResponseTemplate =
"""
using FluentValidation;
using MyHomeRamen.Features.Common.Endpoints.{CQRS};
using MyHomeRamen.Features.{Module}.Features.Abstractions;
using MyHomeRamen.Features.{Module}.Features.{Aggregate}.Common;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{Feature};

public sealed record {FEATURE}{CQRS}(/*ToDo: complete request shape*/);

{DbQueryParams}{AUTH_POLICY}{VALIDATOR}
public sealed class {FEATURE}Handler(I{Module}DbContext dbContext) : I{CQRS}Handler<{FEATURE}{CQRS}, {FEATURE}Response>
{
    public async Task<{FEATURE}Response> Handle({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
        return new {FEATURE}Response();
    }
}
""";

private const string _cqrsHandlerWithoutResponseTemplate =
"""
using FluentValidation;
using MyHomeRamen.Features.Common.Endpoints.{CQRS};
using MyHomeRamen.Features.{Module}.Features.Abstractions;
using MyHomeRamen.Features.{Module}.Features.{Aggregate}.Common;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{Feature};

public sealed record {FEATURE}{CQRS}(/*ToDo: complete request shape*/);

{DbQueryParams}{AUTH_POLICY}{VALIDATOR}
public sealed class {FEATURE}Handler(I{Module}DbContext dbContext) : I{CQRS}Handler<{FEATURE}{CQRS}>
{
    public async Task Handle({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
    }
}
""";

private const string _dbQueryParamsTemplate =
"""
/*public sealed record {FEATURE}QueryOptions()
                     : DbQueryOptions<TEntity, TDto>
(
    //ToDo: complete query options including setting proper generic types for TEntity and TDto
);*/

""";

private const string _validatorTemplate =
"""
public sealed class {FEATURE}Validator : AbstractValidator<{FEATURE}{CQRS}>
{
    //ToDo: implement validation rules
    public {FEATURE}Validator()
    {
    }
}

""";


private const string _cqrsAuthPolicyWithResponseTemplate =
"""
public sealed class {FEATURE}AuthorizationPolicy() : IAuthorizationPolicy<{FEATURE}{CQRS}, {FEATURE}Response>
{
    public async Task<bool> Authorize({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
        return false;
    }
}

""";

private const string _cqrsAuthPolicyWithoutResponseTemplate =
"""
public sealed class {FEATURE}AuthorizationPolicy() : IAuthorizationPolicy<{FEATURE}{CQRS}>
{
    public async Task<bool> Authorize({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
        return false;
    }
}

""";

public static string CreateEndpoint(FeatureDetails featureDetails)
{
    bool hasResponse = featureDetails.Endpoint.Type == "Get"
                    || featureDetails.Endpoint.Type == "Post"
                    || featureDetails.Constructors.Any(c => c.EndsWith("Response"));

    string template = hasResponse ? _withResponseEndpointTemplate : _withoutResponseEndpointTemplate;

    string requestContract = CalculateRequestContractTemplate(featureDetails);
    string responseContract = hasResponse ? CalculateResponseContractTemplate(featureDetails) : string.Empty;

    template = template.Replace("{REQUEST_CONTRACT}", requestContract)
                       .Replace("{RESPONSE_CONTRACT}", responseContract);   

    return ReplacePlaceholders(template, featureDetails);
}

public static string CreateCommand(FeatureDetails featureDetails)
{
    bool hasResponse = featureDetails.Endpoint.Type == "Get"
                    || featureDetails.Endpoint.Type == "Post"
                    || featureDetails.Constructors.Any(c => c.EndsWith("Response"));

    string template = hasResponse ? _cqrsHandlerWithResponseTemplate : _cqrsHandlerWithoutResponseTemplate;

    string authPolicyTemplate = featureDetails.Command.hasAuthPolicy
                              ? hasResponse ? _cqrsAuthPolicyWithResponseTemplate : _cqrsAuthPolicyWithoutResponseTemplate
                              : string.Empty;

    string dbQueryParamsTemplate = featureDetails.Command.hasDbQueryParams
                                 ? _dbQueryParamsTemplate
                                 : string.Empty;

    string validatorTemplate = featureDetails.Command.hasValidator
                              ? _validatorTemplate
                              : string.Empty;

    template = template.Replace("{DbQueryParams}", dbQueryParamsTemplate)
                       .Replace("{AUTH_POLICY}", authPolicyTemplate)
                       .Replace("{VALIDATOR}", validatorTemplate);

    return ReplacePlaceholders(template, featureDetails);
}

private static string CalculateRequestContractTemplate(FeatureDetails featureDetails)
{
    string? requestConstructor = featureDetails.Constructors.FirstOrDefault(c => c.EndsWith("Request"));
    
    List<string> requestDtos = [];
    
    if (requestConstructor != null)
    {
        requestDtos = GetContractDtos(requestConstructor);
        requestDtos.Add(requestConstructor);
    }
    else
    {
        requestDtos.Add("public sealed record {FEATURE}Request(/*ToDo: Complete feature request contract*/)");
    }    

    return string.Join(Environment.NewLine, requestDtos);
}

private static string CalculateResponseContractTemplate(FeatureDetails featureDetails)
{
    string? responseConstructor = featureDetails.Constructors.FirstOrDefault(c => c.EndsWith("Response"));

    List<string> responseDtos = [];

    if (responseConstructor != null)
    {
        responseDtos = GetContractDtos(responseConstructor);
        responseDtos.Add(responseConstructor);
    }
    else
    {
        responseDtos.Add("public sealed record {FEATURE}Response(/*ToDo: Complete feature response contract*/)");
    }

    return string.Join(Environment.NewLine, responseDtos);
}

private static List<string> GetContractDtos(string contract, IEnumerable<string> availableContracts)
{
    List<string> result = [];
    List<string> contractParameters = ExtractContractParameters(contract);

    foreach (string contractParameter in contractParameters)
    {
        string? nestedDto = availableContracts.FirstOrDefault(c => c.Contains($"sealed record {contractParameter}"));
        
        if (nestedDto is not null)
        {
            List<string> nestedDtos = GetContractDtos(nestedDto ?? string.Empty, availableContracts).ToList();

            if (nestedDtos.Any())
            {
                result.AddRange(nestedDtos);
            }

            result.Add(nestedDto);
        }
    }

    return result.Distinct();
}

private static List<string> ExtractContractParameters(string contract)
{
    string sanitazedContract = contract.Replace("\n", " ");

    return sanitazedContract.Substring(sanitazedContract.IndexOf('(') + 1)
                            .Replace(")", "")
                            .Split(',')
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrEmpty(p))
                            .ToList();
}

private static string ReplacePlaceholders(string template, FeatureDetails featureDetails)
{
    return template.Replace("{ENDPOINT_TYPE}", featureDetails.Endpoint.Type)
                   .Replace("{ROUTE}", featureDetails.Endpoint.Route);
                   .Replace("{FEATURE}", featureDetails.Name)
                   .Replace("{Aggregate}", featureDetails.Aggregate)
                   .Replace("{Module}", featureDetails.Module)
                   .Replace("{CQRS}", featureDetails.Command.Type)
                   .Replace("{CQRS_LOWERCASE}", featureDetails.Command.Type.ToLower());
}
