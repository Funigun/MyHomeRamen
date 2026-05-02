namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

public sealed record BasketItemProductDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<BasketItemIngredientDto> BaseIngredients,
    IEnumerable<BasketItemIngredientDto> CustomIngredients);
