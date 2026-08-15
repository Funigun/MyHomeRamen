namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

public sealed record BasketDetailsItemDto(
    Guid Id,
    int Quantity,
    decimal Price,
    string? Comment,
    BasketDetailsItemProductDto Product);
