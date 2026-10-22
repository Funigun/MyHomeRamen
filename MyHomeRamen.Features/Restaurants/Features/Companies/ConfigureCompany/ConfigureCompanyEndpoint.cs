using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.ConfigureCompany;

public sealed class ConfigureCompanyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<ConfigureCompanyResponse>("api/restaurants/company/setup/company", HandleAsync)
                       .WithName("ConfigureCompany")
                       .WithTags("Restaurants", "Company setup")
                       .WithDescription("Configures the initial company.")
                       .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfigureCompanyRequest request,
        [FromServices] IRequestHandler<ConfigureCompanyCommand, ConfigureCompanyResponse> handler,
        CancellationToken cancellationToken)
    {
        ConfigureCompanyResponse response = await handler.Handle(
            new ConfigureCompanyCommand(request),
            cancellationToken);

        return Results.Created("api/restaurants/company/setup/status", response);
    }
}
