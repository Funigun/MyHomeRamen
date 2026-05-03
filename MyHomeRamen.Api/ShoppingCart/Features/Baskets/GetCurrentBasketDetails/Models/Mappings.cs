using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

internal static class Mappings
{
    public static GetCurrentBasketDetailsResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(i => i.ToDto()));

    private static BasketItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Quantity,
            item.Price,
            item.Comment,
            item.Product.ToProductDto());

    private static BasketItemProductDto ToProductDto(this Product product)
        => new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new BasketItemIngredientDto(i.Id.Value, i.Name)),
            product.CustomIngredients.Select(i => new BasketItemIngredientDto(i.Id.Value, i.Name)));
}
