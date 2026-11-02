using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

public interface IRestaurantIngredientPriceLoader
{
    Task<RestaurantIngredientPrice?> ByIngredientId(
        IngredientId ingredientId,
        CancellationToken cancellationToken);
}
