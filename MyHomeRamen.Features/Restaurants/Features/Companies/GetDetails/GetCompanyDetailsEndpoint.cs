using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.GetDetails;

public sealed record BusinessDetailsDto(string LegalName, string TaxId);

public sealed record AllowedActionsDto(bool CanEditCompanyDetails);

public sealed record GetCompanyDetailsResponse(
    Guid Id,
    string Name,
    string? Description,
    string? LogoUrl,
    BusinessDetailsDto BusinessDetails,
    AllowedActionsDto AllowedActions);

public sealed class GetCompanyDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetCompanyDetailsResponse>("api/restaurants/company/details", HandleAsync)
                       .WithName("GetCompanyDetails")
                       .WithDescription("Returns current company details.")
                       .WithTags("restaurants", "companies")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCompanyDetailsQuery, GetCompanyDetailsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCompanyDetailsResponse response = await handler.Handle(new GetCompanyDetailsQuery(), cancellationToken);
        return TypedResults.Ok(response);
    }
}
