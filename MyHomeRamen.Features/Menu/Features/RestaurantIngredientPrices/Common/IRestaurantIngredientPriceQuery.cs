using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

public interface IRestaurantIngredientPriceQuery
{
    Task<MyHomeRamen.Features.Common.Endpoints.Models.PagedResult<RestaurantIngredientPriceForManageDto>> ForManage(
        GetRestaurantIngredientPricesForManageQueryOptions options,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RestaurantIngredientPriceDto>> ByIngredientIds(
        IEnumerable<IngredientId> ingredientIds,
        CancellationToken cancellationToken);
}
