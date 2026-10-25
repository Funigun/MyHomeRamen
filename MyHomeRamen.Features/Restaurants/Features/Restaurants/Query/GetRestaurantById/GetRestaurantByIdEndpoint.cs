using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Mediator;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantById;

public sealed class GetRestaurantByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder) 
        => endpointBuilder.MapStandardGet<GetRestaurantByIdResponse>("api/restaurants/restaurants/{restaurantId:guid}", HandleAsync)
                          .WithName("GetRestaurantById")
                          .WithTags("Restaurants")
                          .WithDescription("Gets restaurant details.")
                          .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    private static async Task<Ok<GetRestaurantByIdResponse>> HandleAsync(Guid restaurantId, [FromServices] IRequestHandler<GetRestaurantByIdQuery, GetRestaurantByIdResponse> handler, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.Handle(new GetRestaurantByIdQuery(restaurantId), cancellationToken));  
    }
}

