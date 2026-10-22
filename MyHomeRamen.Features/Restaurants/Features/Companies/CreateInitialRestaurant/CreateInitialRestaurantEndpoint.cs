using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.CreateInitialRestaurant;

public sealed class CreateInitialRestaurantEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateInitialRestaurantResponse>("api/restaurants/company/setup/restaurant", HandleAsync)
                       .WithName("CreateInitialRestaurant")
                       .WithTags("Restaurants", "Company setup")
                       .WithDescription("Creates the initial restaurant after owner and company setup.")
                       .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateInitialRestaurantRequest request,
        [FromServices] IRequestHandler<CreateInitialRestaurantCommand, CreateInitialRestaurantResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateInitialRestaurantResponse response = await handler.Handle(
            new CreateInitialRestaurantCommand(request),
            cancellationToken);

        return Results.Created($"api/restaurants/restaurants/{response.RestaurantId}", response);
    }
}
