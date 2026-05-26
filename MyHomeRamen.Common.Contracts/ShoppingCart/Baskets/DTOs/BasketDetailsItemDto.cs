namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

public sealed record BasketDetailsItemDto(
    Guid Id,
    int Quantity,
    decimal Price,
    string? Comment,
    BasketDetailsItemProductDto Product);
