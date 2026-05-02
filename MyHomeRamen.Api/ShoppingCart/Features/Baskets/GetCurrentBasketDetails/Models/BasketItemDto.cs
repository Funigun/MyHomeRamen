namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

public sealed record BasketItemDto(
    Guid Id,
    int Quantity,
    decimal Price,
    string? Comment,
    BasketItemProductDto Product);
