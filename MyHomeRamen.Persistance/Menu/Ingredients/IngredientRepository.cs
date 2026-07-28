using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class IngredientRepository(MenuDbContext menuDbContext) : BaseRepository<Ingredient, IngredientId>(menuDbContext), IIngredientRepository
{
    IIngredientQuery IIngredientRepository.Query() => this;

    IIngredientSpecification IIngredientRepository.Specification() => this;
}
