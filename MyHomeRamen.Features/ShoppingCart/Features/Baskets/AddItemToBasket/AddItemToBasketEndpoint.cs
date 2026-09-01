using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<BasketIngredientDto> BaseIngredients,
    List<BasketIngredientDto> CustomIngredients,
    string? Comments);

public sealed record BasketIngredientDto(Guid Id, int Quantity);

public sealed record AddItemToBasketResponse(Guid BasketId, Guid BasketItemId);

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
        [FromServices] IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse> handler,
        CancellationToken cancellationToken)
    {
        AddItemToBasketCommand command = new(request);
        AddItemToBasketResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/shoppingcart/basket/items/{response.BasketItemId}", response);
    }
}
