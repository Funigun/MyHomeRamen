using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantOptions;

public sealed record GetRestaurantOptionsResponse(IEnumerable<RestaurantOptionDto> Restaurants);

public sealed class GetRestaurantOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
        => endpointBuilder.MapStandardGet<GetRestaurantOptionsResponse>("api/restaurants/options", HandleAsync)
                          .WithName("GetRestaurantOptions")
                          .WithTags("Restaurants")
                          .WithDescription("Gets active restaurants available to customers.")
                          .AllowAnonymous();

    private static async Task<Ok<GetRestaurantOptionsResponse>> HandleAsync(
        [FromServices] IRequestHandler<GetRestaurantOptionsQuery, GetRestaurantOptionsResponse> handler,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await handler.Handle(new GetRestaurantOptionsQuery(), cancellationToken));
}
