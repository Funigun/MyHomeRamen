using System.Linq.Expressions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class IngredientRepository : IIngredientQuery
{
    public async Task<List<Ingredient>> GetForDropdown(CancellationToken cancellationToken)
        => (await QueryList(DbQueryOptions<Ingredient>.Empty.OrderByAsc(i => i.Name), i => i, cancellationToken)).ToList();

    public async Task<List<Ingredient>> GetForManage(string? name, IEnumerable<Guid>? categoryIds, CancellationToken cancellationToken)
    {
        string? nameFilter = string.IsNullOrWhiteSpace(name) ? null : name.ToLower();

        List<CategoryId>? ids = categoryIds is not null && categoryIds.Any()
            ? categoryIds.Select(id => (CategoryId)id).ToList()
            : null;

        Expression<Func<Ingredient, bool>> predicate = i =>
            (nameFilter == null || i.Name.ToLower().Contains(nameFilter)) &&
            (ids == null || i.Categories.Any(c => ids.Contains(c.Id)));

        return (await QueryList(
            DbQueryOptions<Ingredient>.Where(predicate).OrderByAsc(i => i.Name),
            i => i,
            cancellationToken)).ToList();
    }

    public async Task<List<Ingredient>> GetByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken)
        => (await QueryList(DbQueryOptions<Ingredient>.Where(i => ingredientIds.Contains(i.Id)), i => i, cancellationToken)).ToList();

    public async Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(i => i.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken)
        => !await Exists(i => i.Id != excludeId && i.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsCategoryUsedByIngredient(CategoryId categoryId, CancellationToken cancellationToken)
        => await Exists(i => i.Categories.Any(c => c.Id == categoryId), cancellationToken);
}
