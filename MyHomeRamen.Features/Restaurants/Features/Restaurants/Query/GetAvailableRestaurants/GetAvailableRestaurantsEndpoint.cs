using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Mediator;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetAvailableRestaurants;

public sealed class GetAvailableRestaurantsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder) 
        => endpointBuilder.MapStandardGet<GetAvailableRestaurantsResponse>("api/restaurants/restaurants", HandleAsync)
                          .WithName("GetAvailableRestaurants")
                          .WithTags("Restaurants")
                          .WithDescription("Gets available restaurants.")
                          .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    private static async Task<Ok<GetAvailableRestaurantsResponse>> HandleAsync([FromServices] IRequestHandler<GetAvailableRestaurantsQuery, GetAvailableRestaurantsResponse> handler, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await handler.Handle(new GetAvailableRestaurantsQuery(), cancellationToken));
    }
}
