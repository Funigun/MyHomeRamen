using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class IngredientRepository(MenuDbContext menuDbContext, ICacheService cacheService) : BaseRepository<Ingredient, IngredientId>(menuDbContext, cacheService), IIngredientRepository
{
    IIngredientQuery IIngredientRepository.Query() => this;

    IIngredientLoader IIngredientRepository.Load() => this;
}
