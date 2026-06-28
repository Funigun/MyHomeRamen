using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Ingredient> ingredients)
    {
        public IQueryable<Ingredient> ForDropdown()
            => ingredients.AsNoTracking().OrderBy(i => i.Name);

        public async Task<IEnumerable<Ingredient>> GetByIds(IEnumerable<IngredientId> keys, CancellationToken cancellationToken)
            => await ingredients.Where(e => keys.Contains(e.Id)).ToListAsync(cancellationToken);

        public IQueryable<Ingredient> ForManage(string? name, IEnumerable<Guid>? categoryIds)
        {
            IQueryable<Ingredient> query = ingredients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(i => i.Name.ToLower().Contains(name.ToLower()));
            }

            if (categoryIds is not null && categoryIds.Any())
            {
                List<CategoryId> ids = categoryIds.Select(id => (CategoryId)id).ToList();
                query = query.Where(i => i.Categories.Any(c => ids.Contains(c.Id)));
            }

            return query.OrderBy(i => i.Name);
        }

        public async Task<bool> IsIngredientNameUniqueAsync(string name, CancellationToken cancellationToken = default)
            => await ingredients.AsNoTracking().AnyAsync(i => i.Name.ToLower() != name.ToLower(), cancellationToken);

        public async Task<bool> IsIngredientNameUniqueExcludingAsync(string name, IngredientId excludeId, CancellationToken cancellationToken = default)
            => !await ingredients.AsNoTracking().AnyAsync(i => i.Id != excludeId && i.Name.ToLower() == name.ToLower(), cancellationToken);

        public async Task<bool> IsCategoryUsedByIngredientAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
            => await ingredients.AsNoTracking().AnyAsync(i => i.Categories.Any(c => c.Id == categoryId), cancellationToken);
    }
}
