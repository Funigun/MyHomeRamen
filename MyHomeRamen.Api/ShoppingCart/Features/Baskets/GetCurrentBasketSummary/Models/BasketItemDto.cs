namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary.Models;

public sealed record BasketItemDto(
    Guid Id,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal Price);
