using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class RestaurantIngredientPriceRepository : IRestaurantIngredientPriceLoader
{
    public async Task<RestaurantIngredientPrice?> ByIngredientId(
        IngredientId ingredientId,
        CancellationToken cancellationToken)
        => await menuDbContext.RestaurantIngredientPrices
            .FirstOrDefaultAsync(price => price.IngredientId == ingredientId, cancellationToken);
}
