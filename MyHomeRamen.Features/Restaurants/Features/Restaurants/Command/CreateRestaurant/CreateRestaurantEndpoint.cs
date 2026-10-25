using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Mediator;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Command.CreateRestaurant;

public sealed class CreateRestaurantEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateRestaurantResponse>("api/restaurants/restaurants", HandleAsync)
                       .WithName("CreateRestaurant")
                       .WithTags("Restaurants")
                       .WithDescription("Creates restaurant.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync([FromBody] CreateRestaurantRequest request, [FromServices] IRequestHandler<CreateRestaurantCommand, CreateRestaurantResponse> handler, CancellationToken cancellationToken)
    {
        CreateRestaurantResponse response = await handler.Handle(new CreateRestaurantCommand(request), cancellationToken);
        return Results.Created($"/api/restaurants/restaurants/{response.Id}", response);
    }
}

