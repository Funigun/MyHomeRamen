using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class IngredientRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<Ingredient, IngredientId>(shoppingCartDbContext, cacheService), IIngredientRepository
{
    IIngredientQuery IIngredientRepository.Query() => this;

    IIngredientLoader IIngredientRepository.Load() => this;
}
