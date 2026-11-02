using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;

public class CheckoutBasketItemModel
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid OfferId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? CurrentPrice { get; set; }

    public bool IsPriceChanged { get; set; }

    public bool IsAvailable { get; set; }

    public string? Comment { get; set; }

    public CheckoutProductModel Product { get; set; } = new();

    public static CheckoutBasketItemModel FromDetailsDto(BasketDetailsItemDto basketDetailsItemDto)
    {
        return new()
        {
            Id = basketDetailsItemDto.Id,
            RestaurantId = basketDetailsItemDto.RestaurantId,
            OfferId = basketDetailsItemDto.OfferId,
            Quantity = basketDetailsItemDto.Quantity,
            Price = basketDetailsItemDto.Price,
            CurrentPrice = basketDetailsItemDto.CurrentPrice,
            IsPriceChanged = basketDetailsItemDto.IsPriceChanged,
            IsAvailable = basketDetailsItemDto.IsAvailable,
            Comment = basketDetailsItemDto.Comment,
            Product = CheckoutProductModel.FromDetailsDto(basketDetailsItemDto.Product)
        };
    }

    public static CheckoutBasketItemModel FromAddItemResponse(AddItemToBasketResponse response, CheckoutBasketItemModel source)
    {
        return new()
        {
            Id = response.BasketItemId,
            RestaurantId = source.RestaurantId,
            OfferId = source.OfferId,
            Quantity = source.Quantity,
            Price = source.Price,
            CurrentPrice = source.CurrentPrice,
            IsPriceChanged = source.IsPriceChanged,
            IsAvailable = source.IsAvailable,
            Comment = source.Comment,
            Product = source.Product
        };
    }
}
