using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;

public sealed record AddItemToBasketRequest(
    Guid RestaurantId,
    Guid OfferId,
    int Quantity,
    List<Guid> ExtraIngredientIds,
    string? Comments,
    bool ReplaceExistingBasket = false);
