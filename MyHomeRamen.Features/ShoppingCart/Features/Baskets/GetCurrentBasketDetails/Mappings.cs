using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

internal static class Mappings
{
    public static GetCurrentBasketDetailsResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(i => i.ToDto()));

    private static BasketDetailsItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Quantity,
            item.Price,
            item.Comment,
            item.Product.ToProductDto());

    private static BasketDetailsItemProductDto ToProductDto(this Product product)
        => new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.Name, i.Description, i.Price)),
            product.CustomIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.Name, i.Description, i.Price)));
}

