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
                ShoppingCartDataSet.RestaurantId,
                item.RestaurantProductOfferId,
                item.Quantity,
                item.SelectedExtraIngredientIds.Select(id => id.Value).ToList(),
                item.Comment
            );
        }
    }
}
