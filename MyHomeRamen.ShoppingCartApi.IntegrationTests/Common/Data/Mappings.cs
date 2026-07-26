using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;

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
                item.Product.BaseIngredients.Select(i => new BasketIngredientDto(i.Id.Value, i.Quantity)).ToList(),
                item.Product.CustomIngredients.Select(i => new BasketIngredientDto(i.Id.Value, i.Quantity)).ToList(),
                item.Comment
            );
        }
    }

    extension(Ingredient ingredient)
    {
        internal BasketIngredientDto ToBasketIngredientDto()
        {
            return new BasketIngredientDto(ingredient.Id.Value, 1);
        }
    }
}
