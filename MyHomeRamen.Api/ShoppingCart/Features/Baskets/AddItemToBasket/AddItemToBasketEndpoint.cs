using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPost<AddItemToBasketRequest, AddItemToBasketResponse>(
                "basket/items", HandleAsync)
            .WithName("AddItemToBasketEndpoint")
            .WithDescription("Adds a product with selected ingredients to the current user's basket.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AddItemToBasketRequest request,
        [FromServices] IRequestHandler<AddItemToBasketRequest, AddItemToBasketResponse> handler,
        CancellationToken cancellationToken)
    {
        AddItemToBasketResponse response = await handler.Handle(request, cancellationToken);
        return Results.Created($"/api/shoppingcart/basket/items/{response.BasketItemId}", response);
    }
}
