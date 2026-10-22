using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Setup;

public sealed class GetCompanySetupStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<CompanySetupStatusResponse>("api/restaurants/company/setup/status", HandleAsync)
                       .WithName("GetCompanySetupStatus")
                       .WithTags("Restaurants", "Company setup")
                       .WithDescription("Returns progress for owner, company, and initial restaurant setup.")
                       .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCompanySetupStatusQuery, CompanySetupStatusResponse> handler,
        CancellationToken cancellationToken)
    {
        CompanySetupStatusResponse response = await handler.Handle(
            new GetCompanySetupStatusQuery(),
            cancellationToken);

        return Results.Ok(response);
    }
}
