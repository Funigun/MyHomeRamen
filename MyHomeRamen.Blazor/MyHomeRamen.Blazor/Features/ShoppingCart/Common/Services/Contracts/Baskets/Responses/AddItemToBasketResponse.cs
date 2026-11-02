namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

public sealed record AddItemToBasketResponse(
    Guid BasketId,
    Guid BasketItemId,
    bool RequiresBasketReplacement = false,
    Guid? CurrentRestaurantId = null);
