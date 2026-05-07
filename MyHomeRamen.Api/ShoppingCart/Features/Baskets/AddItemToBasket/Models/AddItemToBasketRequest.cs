using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;

public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<IngredientRequestDto> BaseIngredients,
    List<IngredientRequestDto> CustomIngredients,
    string? Comments) : IRequest<AddItemToBasketResponse>;
