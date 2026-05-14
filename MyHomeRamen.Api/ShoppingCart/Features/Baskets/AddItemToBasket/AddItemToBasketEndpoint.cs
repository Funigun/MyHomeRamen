using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPost<AddItemToBasketCommand, AddItemToBasketResponse>(
                "api/shoppingcart/basket/items", HandleAsync)
            .WithName("AddItemToBasketEndpoint")
            .WithTags("Baskets")
            .WithDescription("Adds a product with selected ingredients to the current user's basket.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AddItemToBasketRequest request,
        [FromServices] IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse> handler,
        CancellationToken cancellationToken)
    {
        AddItemToBasketCommand command = new(request);
        AddItemToBasketResponse response = await handler.Handle(command, cancellationToken);
        return Results.Created($"/api/shoppingcart/basket/items/{response.BasketItemId}", response);
    }
}
