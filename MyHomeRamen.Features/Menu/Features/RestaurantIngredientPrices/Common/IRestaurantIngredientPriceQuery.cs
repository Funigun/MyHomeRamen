using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

public interface IRestaurantIngredientPriceQuery
{
    Task<IReadOnlyCollection<RestaurantIngredientPriceDto>> ByIngredientIds(
        IEnumerable<IngredientId> ingredientIds,
        CancellationToken cancellationToken);
}
