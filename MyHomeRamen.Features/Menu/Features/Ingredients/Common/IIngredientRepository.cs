using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public interface IIngredientRepository : IRepository<Ingredient, IngredientId>
{
    IIngredientQuery Query();

    IIngredientLoader Load();
}
