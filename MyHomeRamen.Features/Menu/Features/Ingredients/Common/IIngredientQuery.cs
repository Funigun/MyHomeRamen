using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public interface IIngredientQuery
{
    Task<List<Ingredient>> GetForDropdown(CancellationToken cancellationToken = default);

    Task<List<Ingredient>> GetForManage(string? name, IEnumerable<Guid>? categoryIds, CancellationToken cancellationToken = default);

    Task<List<Ingredient>> GetByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken = default);

    Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken = default);

    Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken = default);

    Task<bool> IsCategoryUsedByIngredient(CategoryId categoryId, CancellationToken cancellationToken = default);
}