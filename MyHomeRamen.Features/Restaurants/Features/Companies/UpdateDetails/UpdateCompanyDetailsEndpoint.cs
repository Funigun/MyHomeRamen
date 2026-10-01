using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.UpdateDetails;

public sealed record BusinessDetailsForUpdateDto(string LegalName, string TaxId);

public sealed record UpdateCompanyDetailsRequest(
    string? Description,
    string? LogoUrl,
    BusinessDetailsForUpdateDto BusinessDetails);

public sealed class UpdateCompanyDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut("api/restaurants/company/details", HandleAsync)
                       .WithName("UpdateCompanyDetails")
                       .WithDescription("Updates current company details.")
                       .WithTags("restaurants", "companies")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateCompanyDetailsRequest request,
        [FromServices] IRequestHandler<UpdateCompanyDetailsCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new UpdateCompanyDetailsCommand(request), cancellationToken);
        return TypedResults.NoContent();
    }
}
