namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

public sealed record BasketDetailsItemProductDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<BasketDetailsIngredientDto> BaseIngredients,
    IEnumerable<BasketDetailsIngredientDto> CustomIngredients);
