using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public interface IIngredientQuery
{
    Task<List<Ingredient>> GetForDropdown(CancellationToken cancellationToken);

    Task<List<Ingredient>> GetForManage(string? name, IEnumerable<Guid>? categoryIds, CancellationToken cancellationToken);

    Task<List<Ingredient>> GetByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken);

    Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken);

    Task<bool> IsCategoryUsedByIngredient(CategoryId categoryId, CancellationToken cancellationToken);
}