#:include FeatureDetails.cs

private const string _withResponseEndpointTemplate =
"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{FEATURE};

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
        [FromServices] IRequestHandler<{FEATURE}{CQRS}, {FEATURE}Response> handler,
        CancellationToken cancellationToken)
    {
        /*Todo: add request parameters and pass to {CQRS_LOWERCASE} if needed*/
        {FEATURE}{CQRS} {CQRS_LOWERCASE} = new();
        {FEATURE}Response response = await handler.Handle({CQRS_LOWERCASE}, cancellationToken);

        return TypedResults.Ok(response);
    }
}
""";

private const string _cqrsTemplate =
"""
using FluentValidation;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.{Module}.Features.Abstractions;
using MyHomeRamen.Features.{Module}.Features.{Aggregate}.Common;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{Feature};

public sealed record {FEATURE}{CQRS}(/*ToDo: complete request shape*/);

public sealed record {FEATURE}Dto(/*ToDo: complete DTO shape*/);

/*public sealed record {FEATURE}{CQRS}Options()
                     : DbQueryOptions<TEntity, {FEATURE}Dto>
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

public sealed class {FEATURE}Handler(I{Module}DbContext dbContext)
    : IRequestHandler<{FEATURE}{CQRS}, {FEATURE}Response>
{
    public async Task<{FEATURE}Response> Handle({FEATURE}{CQRS} {CQRS_LOWERCASE}, CancellationToken cancellationToken)
    {
        //ToDo: implement handler logic
        return new {FEATURE}Response();
    }
}
""";

public static string CreateEndpoint(FeatureDetails featureDetails)
{
    string requestContract = CalculateRequestContractTemplate(featureDetails);
    string responseContract = CalculateResponseContractTemplate(featureDetails);

    string template = _withResponseEndpointTemplate
        .Replace("{REQUEST_CONTRACT}", requestContract)
        .Replace("{RESPONSE_CONTRACT}", responseContract);

    return ReplacePlaceholders(template, featureDetails);
}

public static string CreateCqrs(FeatureDetails featureDetails)
{
    return ReplacePlaceholders(_cqrsTemplate, featureDetails);
}

private static string CalculateRequestContractTemplate(FeatureDetails featureDetails)
{
    string? requestConstructor = featureDetails.Constructors.FirstOrDefault(
        c => GetRecordName(c).Equals($"{featureDetails.Name}Request", StringComparison.Ordinal));
    
    List<string> requestDtos = [];
    
    if (requestConstructor != null)
    {
        requestDtos = GetContractDtos(requestConstructor, featureDetails.Constructors);
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
    string? responseConstructor = featureDetails.Constructors.FirstOrDefault(
        c => GetRecordName(c).Equals($"{featureDetails.Name}Response", StringComparison.Ordinal));

    List<string> responseDtos = [];

    if (responseConstructor != null)
    {
        responseDtos = GetContractDtos(responseConstructor, featureDetails.Constructors);
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
        string parameterType = contractParameter
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? string.Empty;
        string? nestedDto = availableContracts.FirstOrDefault(
            c => GetRecordName(c).Equals(parameterType, StringComparison.Ordinal));
        
        if (nestedDto is not null)
        {
            List<string> nestedDtos = GetContractDtos(nestedDto, availableContracts).ToList();

            if (nestedDtos.Any())
            {
                result.AddRange(nestedDtos);
            }

            result.Add(nestedDto);
        }
    }

    return result.Distinct().ToList();
}

private static string GetRecordName(string declaration)
{
    int recordIndex = declaration.IndexOf("record ", StringComparison.Ordinal);
    if (recordIndex < 0)
    {
        return string.Empty;
    }

    int nameStart = recordIndex + "record ".Length;
    int nameEnd = declaration.IndexOf('(', nameStart);
    if (nameEnd < 0)
    {
        nameEnd = declaration.Length;
    }

    return declaration[nameStart..nameEnd].Trim();
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
                   .Replace("{ROUTE}", featureDetails.Endpoint.Route ?? "\"/TODO\"")
                   .Replace("{FEATURE}", featureDetails.Name)
                   .Replace("{Aggregate}", featureDetails.Aggregate)
                   .Replace("{Module}", featureDetails.Module)
                   .Replace("{CQRS}", featureDetails.Command.Type)
                   .Replace("{CQRS_LOWERCASE}", featureDetails.Command.Type.ToLower());
}
