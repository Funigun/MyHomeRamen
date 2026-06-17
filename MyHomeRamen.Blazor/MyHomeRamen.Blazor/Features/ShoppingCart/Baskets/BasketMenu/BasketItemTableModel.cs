using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.BasketMenu;

public sealed class BasketItemTableModel
{
    public Guid Id { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public string ProductImageUrl { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal Price { get; init; }

    public static BasketItemTableModel FromResponse(BasketSummaryItemDto response)
    {
        return new BasketItemTableModel
        {
            Id = response.Id,
            ProductName = response.ProductName,
            ProductImageUrl = response.ProductImageUrl,
            Quantity = response.Quantity,
            Price = response.Price,
        };
    }
}
