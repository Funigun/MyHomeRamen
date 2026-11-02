namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

public sealed record BasketDetailsItemProductDto(
    Guid Id,
    Guid OriginalId,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<BasketDetailsIngredientDto> BaseIngredients,
    IEnumerable<BasketDetailsIngredientDto> CustomIngredients);
