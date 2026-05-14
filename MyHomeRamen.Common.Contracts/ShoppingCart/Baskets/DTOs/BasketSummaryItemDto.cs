namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

public sealed record BasketSummaryItemDto(
    Guid Id,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal Price);
