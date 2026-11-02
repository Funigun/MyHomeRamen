namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

public sealed record BasketDetailsItemDto(
    Guid Id,
    Guid RestaurantId,
    Guid OfferId,
    int Quantity,
    decimal Price,
    decimal? CurrentPrice,
    bool IsPriceChanged,
    bool IsAvailable,
    string? Comment,
    BasketDetailsItemProductDto Product);
