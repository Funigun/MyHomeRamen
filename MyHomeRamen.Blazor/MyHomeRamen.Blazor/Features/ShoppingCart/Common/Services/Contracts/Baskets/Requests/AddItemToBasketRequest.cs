using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;

public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<BasketIngredientDto> BaseIngredients,
    List<BasketIngredientDto> CustomIngredients,
    string? Comments);
