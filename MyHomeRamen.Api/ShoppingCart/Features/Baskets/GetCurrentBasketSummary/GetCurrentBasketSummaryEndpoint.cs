using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed class GetCurrentBasketSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketSummaryResponse>("api/shoppingcart/basket/summary", HandleAsync)
            .WithName("GetCurrentBasketSummaryEndpoint")
            .WithTags("Baskets")
            .WithDescription("Returns the active basket and its items for shopping cart summary panel.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IQueryHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketSummaryQuery query = new();
        GetCurrentBasketSummaryResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
