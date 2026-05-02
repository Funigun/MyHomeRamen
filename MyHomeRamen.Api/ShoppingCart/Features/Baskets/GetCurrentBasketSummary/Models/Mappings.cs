using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary.Models;

internal static class Mappings
{
    public static GetCurrentBasketSummaryResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(item => item.ToDto()));

    public static BasketItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Product.Name,
            item.Product.ImageUrl,
            item.Quantity,
            item.Price);
}
