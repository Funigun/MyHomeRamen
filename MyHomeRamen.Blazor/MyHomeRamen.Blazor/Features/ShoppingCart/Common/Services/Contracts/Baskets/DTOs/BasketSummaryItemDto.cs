namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

public sealed record BasketSummaryItemDto(
    Guid Id,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal Price);
