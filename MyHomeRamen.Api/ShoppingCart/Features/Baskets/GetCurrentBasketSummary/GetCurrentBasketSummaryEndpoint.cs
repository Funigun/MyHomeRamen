using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed class GetCurrentBasketSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketSummaryResponse>("api/shoppingcart/baskets", HandleAsync)
            .WithName("GetCurrentBasketSummaryEndpoint")
            .WithTags("Baskets")
            .WithDescription("Returns the active basket and its items for the current authenticated user.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketSummaryResponse response = await handler.Handle(new(), cancellationToken);
        return Results.Ok(response);
    }
}
