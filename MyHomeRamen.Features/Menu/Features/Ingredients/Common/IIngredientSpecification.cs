using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public interface IIngredientSpecification
{
    Task<Ingredient> ById(IngredientId ingredientId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Ingredient>> ByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken = default);
}
