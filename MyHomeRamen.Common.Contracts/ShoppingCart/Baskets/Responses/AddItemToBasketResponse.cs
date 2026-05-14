namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

public sealed record AddItemToBasketResponse(Guid BasketId, Guid BasketItemId);
