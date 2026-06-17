using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;

public class CheckoutBasketItemModel
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Comment { get; set; }

    public CheckoutProductModel Product { get; set; } = new();

    public static CheckoutBasketItemModel FromDetailsDto(BasketDetailsItemDto basketDetailsItemDto)
    {
        return new()
        {
            Id = basketDetailsItemDto.Id,
            Quantity = basketDetailsItemDto.Quantity,
            Price = basketDetailsItemDto.Price,
            Comment = basketDetailsItemDto.Comment,
            Product = CheckoutProductModel.FromDetailsDto(basketDetailsItemDto.Product)
        };
    }
}
