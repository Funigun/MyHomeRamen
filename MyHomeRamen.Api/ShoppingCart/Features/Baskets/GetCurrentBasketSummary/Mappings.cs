using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

internal static class Mappings
{
    public static GetCurrentBasketSummaryResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(item => item.ToDto()));

    public static BasketSummaryItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Product.Name,
            item.Product.ImageUrl,
            item.Quantity,
            item.Price);
}
