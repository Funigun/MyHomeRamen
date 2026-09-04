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
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{FEATURE};

{REQUEST_CONTRACT}

{RESPONSE_CONTRACT}

public sealed class {FEATURE}Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandard{ENDPOINT_TYPE}<{FEATURE}Response>($"{ROUTE}", HandleAsync)
            .WithName("{FEATURE}Endpoint")
            .WithTags("/*ToDo: complete tags*/")
            .WithDescription("ToDo: complete description")
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
using MyHomeRamen.Features.{Module}.Abstractions;
using MyHomeRamen.Features.{Module}.{Aggregate}.Common;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.{Module}.Features.{Aggregate}.{FEATURE};

{CQRS_CONTRACT}

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

public sealed class {FEATURE}Handler(I{Module}DbContext dbContext) : IRequestHandler<{FEATURE}{CQRS}, {FEATURE}Response>
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
            .Replace("{REQUEST_CONTRACT}", requestContract, StringComparison.Ordinal)
            .Replace("{RESPONSE_CONTRACT}", responseContract, StringComparison.Ordinal);

        return ReplacePlaceholders(template, featureDetails);
    }

    public static string CreateCqrs(FeatureDetails featureDetails)
    {
        string cqrsContract = CalculateCqrsContractTemplate(featureDetails);

        string template = _cqrsTemplate.Replace(
            "{CQRS_CONTRACT}",
            cqrsContract,
            StringComparison.Ordinal);

        return ReplacePlaceholders(template, featureDetails);
    }

    private static string CalculateRequestContractTemplate(FeatureDetails featureDetails)
    {
        string? requestConstructor = featureDetails.Constructors.FirstOrDefault(c => GetRecordName(c).Equals($"{featureDetails.Name}Request", StringComparison.Ordinal));

        List<string> requestDtos = [];

        if (requestConstructor != null)
        {
            requestDtos = GetContractDtos(requestConstructor, featureDetails.Constructors);
            requestDtos.Add($"{requestConstructor};");
        }
        else
        {
            requestDtos.Add("public sealed record {FEATURE}Request(/*ToDo: Complete feature request contract*/);");
        }

        return string.Join(Environment.NewLine, requestDtos);
    }

    private static string CalculateResponseContractTemplate(FeatureDetails featureDetails)
    {
        string? responseConstructor = featureDetails.Constructors.FirstOrDefault(c => GetRecordName(c).Equals($"{featureDetails.Name}Response", StringComparison.Ordinal));

        List<string> responseDtos = [];

        if (responseConstructor != null)
        {
            responseDtos = GetContractDtos(responseConstructor, featureDetails.Constructors).Select(dto => $"{dto};").ToList();
            responseDtos.Add($"{responseConstructor};");
        }
        else
        {
            responseDtos.Add("public sealed record {FEATURE}Response(/*ToDo: Complete feature response contract*/);");
        }

        return string.Join(Environment.NewLine, responseDtos);
    }

    private static string CalculateCqrsContractTemplate(FeatureDetails featureDetails)
    {
        string cqrsName = $"{featureDetails.Name}{featureDetails.Command.Type}";

        string? cqrsConstructor = featureDetails.Constructors.FirstOrDefault(constructor => GetRecordName(constructor).Equals(cqrsName, StringComparison.Ordinal));

        return $"{cqrsConstructor ?? "public sealed record {FEATURE}{CQRS}(/*ToDo: complete request shape*/)"} : IRequest<{{FEATURE}}Response>;";
    }

    private static List<string> GetContractDtos(string contract,IEnumerable<string> availableContracts)
    {
        List<string> result = [];
        List<string> contractParameters = ExtractContractParameters(contract);

        foreach (string contractParameter in contractParameters)
        {
            string parameterType = contractParameter
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? string.Empty;

            string? nestedDto = availableContracts.FirstOrDefault(
                availableContract =>
                {
                    string recordName = GetRecordName(availableContract);

                    return ExtractTypeNames(parameterType).Any(typeName => typeName.Equals(recordName, StringComparison.Ordinal));
                });

            if (nestedDto is null)
            {
                continue;
            }

            List<string> nestedDtos = GetContractDtos(
                nestedDto,
                availableContracts);

            if (nestedDtos.Count > 0)
            {
                result.AddRange(nestedDtos);
            }

            result.Add(nestedDto);
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
        int nameEnd = declaration.IndexOf('(', nameStart, StringComparison.Ordinal);
        if (nameEnd < 0)
        {
            nameEnd = declaration.Length;
        }

        return declaration[nameStart..nameEnd].Trim();
    }

    private static List<string> ExtractContractParameters(string contract)
    {
        string sanitazedContract = contract.Replace("\n", " ", StringComparison.Ordinal);

        return sanitazedContract[(sanitazedContract.IndexOf('(', StringComparison.Ordinal) + 1)..]
                                .Replace(")", "", StringComparison.Ordinal)
                                .Split(',')
                                .Select(p => p.Trim())
                                .Where(p => !string.IsNullOrEmpty(p))
                                .ToList();
    }

    private static string ReplacePlaceholders(string template, FeatureDetails featureDetails)
    {
        return template.Replace("{ENDPOINT_TYPE}", featureDetails.Endpoint.Type, StringComparison.Ordinal)
                       .Replace("{ROUTE}", featureDetails.Endpoint.Route ?? "\"/TODO\"", StringComparison.Ordinal)
                       .Replace("{FEATURE}", featureDetails.Name, StringComparison.Ordinal)
                       .Replace("{Aggregate}", featureDetails.Aggregate, StringComparison.Ordinal)
                       .Replace("{Module}", featureDetails.Module, StringComparison.Ordinal)
                       .Replace("{CQRS}", featureDetails.Command.Type, StringComparison.Ordinal)
                       .Replace("{CQRS_LOWERCASE}", featureDetails.Command.Type.ToLower(), StringComparison.Ordinal);
    }

    private static IEnumerable<string> ExtractTypeNames(string type)
    {
        int tokenStart = -1;

        for (int index = 0; index <= type.Length; index++)
        {
            bool isIdentifierCharacter =
                index < type.Length &&
                (char.IsLetterOrDigit(type[index]) || type[index] == '_');

            if (isIdentifierCharacter)
            {
                tokenStart = tokenStart < 0 ? index : tokenStart;
                continue;
            }

            if (tokenStart >= 0)
            {
                yield return type[tokenStart..index];
                tokenStart = -1;
            }
        }
    }
}
