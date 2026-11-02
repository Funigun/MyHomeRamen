using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

public interface IRestaurantIngredientPriceRepository :
    IRepository<RestaurantIngredientPrice, RestaurantIngredientPriceId>,
    IRestaurantIngredientPriceQuery
{
    IRestaurantIngredientPriceQuery Query();

    IRestaurantIngredientPriceLoader Load();
}
