namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Responses;

public sealed record AddItemToBasketResponse(Guid BasketId, Guid BasketItemId);
