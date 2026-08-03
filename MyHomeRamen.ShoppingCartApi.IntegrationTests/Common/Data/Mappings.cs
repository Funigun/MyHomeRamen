using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal static class Mappings
{
    extension(BasketItem item)
    {
        internal AddItemToBasketRequest ToAddBasketItemRequest()
        {
            return new AddItemToBasketRequest
            (
                item.Product.OriginalId,
                item.Quantity,
                item.Product.BaseIngredients.Select(i => new Features.ShoppingCart.Features.Baskets.AddItemToBasket.BasketIngredientDto(i.Id.Value, i.Quantity)).ToList(),
                item.Product.CustomIngredients.Select(i => new Features.ShoppingCart.Features.Baskets.AddItemToBasket.BasketIngredientDto(i.Id.Value, i.Quantity)).ToList(),
                item.Comment
            );
        }
    }

    extension(Ingredient ingredient)
    {
        internal Features.ShoppingCart.Features.Baskets.AddItemToBasket.BasketIngredientDto ToBasketIngredientDto()
        {
            return new Features.ShoppingCart.Features.Baskets.AddItemToBasket.BasketIngredientDto(ingredient.Id.Value, 1);
        }
    }
}
