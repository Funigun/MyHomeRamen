using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPost<AddItemToBasketResponse>("api/shoppingcart/basket/items", HandleAsync)
            .WithName("AddItemToBasketEndpoint")
            .WithTags("Baskets")
            .WithDescription("Adds a product with selected ingredients to the current user's basket.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AddItemToBasketRequest request,
        [FromServices] ICommandHandler<AddItemToBasketCommand, AddItemToBasketResponse> handler,
        CancellationToken cancellationToken)
    {
        AddItemToBasketCommand command = new(request);
        AddItemToBasketResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/shoppingcart/basket/items/{response.BasketItemId}", response);
    }
}

