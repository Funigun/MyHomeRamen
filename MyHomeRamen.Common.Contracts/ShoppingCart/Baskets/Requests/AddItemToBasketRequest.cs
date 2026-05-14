using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;

public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<BasketIngredientDto> BaseIngredients,
    List<BasketIngredientDto> CustomIngredients,
    string? Comments);
