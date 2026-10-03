using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Mediator;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Command.AssignManager;

public sealed class AssignRestaurantManagerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
        => endpointBuilder.MapStandardPost<AssignRestaurantManagerResponse>("api/restaurants/restaurants/{restaurantId:guid}/manager", HandleAsync)
                          .WithName("AssignRestaurantManager")
                          .WithTags("Restaurants")
                          .WithDescription("Assigns restaurant manager.")
                          .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    private static async Task<IResult> HandleAsync(Guid restaurantId, [FromBody] AssignRestaurantManagerRequest request, [FromServices] IRequestHandler<AssignRestaurantManagerCommand, AssignRestaurantManagerResponse> handler, CancellationToken cancellationToken)
    {
        AssignRestaurantManagerResponse response = await handler.Handle(new AssignRestaurantManagerCommand(restaurantId, request), cancellationToken);
        return TypedResults.Ok(response);
    }
}

