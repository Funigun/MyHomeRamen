using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;

public sealed class RegisterCompanyOwnerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterCompanyOwnerResponse>("api/restaurants/company/setup/owner", HandleAsync)
                       .WithName("RegisterCompanyOwner")
                       .WithTags("Restaurants", "Company setup")
                       .WithDescription("Registers the initial company owner.")
                       .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterCompanyOwnerRequest request,
        [FromServices] IRequestHandler<RegisterCompanyOwnerCommand, RegisterCompanyOwnerResponse> handler,
        CancellationToken cancellationToken)
    {
        RegisterCompanyOwnerResponse response = await handler.Handle(
            new RegisterCompanyOwnerCommand(request),
            cancellationToken);

        return Results.Created("api/restaurants/company/setup/status", response);
    }
}
