using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IIngredientQuery
{
    private IQueryable<Ingredient> IngredientsQuery => Ingredients.AsNoTracking();

    public async Task<List<Ingredient>> GetForDropdown(CancellationToken cancellationToken)
        => await IngredientsQuery.OrderBy(i => i.Name)
                                 .ToListAsync(cancellationToken);

    public async Task<List<Ingredient>> GetForManage(string? name, IEnumerable<Guid>? categoryIds, CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> query = IngredientsQuery;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(i => i.Name.ToLower().Contains(name.ToLower()));
        }

        if (categoryIds is not null && categoryIds.Any())
        {
            List<CategoryId> ids = categoryIds.Select(id => (CategoryId)id).ToList();
            query = query.Where(i => i.Categories.Any(c => ids.Contains(c.Id)));
        }

        return await query.OrderBy(i => i.Name)
                          .ToListAsync(cancellationToken);
    }

    public async Task<List<Ingredient>> GetByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken)
        => await Ingredients.Where(e => ingredientIds.Contains(e.Id)).ToListAsync(cancellationToken);

    public async Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken)
        => await IngredientsQuery.AnyAsync(i => i.Name.ToLower() != name.ToLower(), cancellationToken);

    public async Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken)
        => !await IngredientsQuery.AnyAsync(i => i.Id != excludeId && i.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsCategoryUsedByIngredient(CategoryId categoryId, CancellationToken cancellationToken)
        => await IngredientsQuery.AnyAsync(i => i.Categories.Any(c => c.Id == categoryId), cancellationToken);
}
