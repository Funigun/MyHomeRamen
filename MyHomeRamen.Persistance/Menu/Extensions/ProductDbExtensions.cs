using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Product> products)
    {
        public IQueryable<Product> ForCategory(CategoryId categoryId)
            => products
                .AsNoTracking()
                .Include(p => p.BaseIngredients)
                .Where(p => p.Categories.Any(c => c.Id == categoryId));

        public IQueryable<Product> WithAllIngredients()
            => products
                .AsNoTracking()
                .Include(p => p.BaseIngredients)
                .Include(p => p.CustomIngredients);

        public IQueryable<Product> ForManage(
            string? name,
            IEnumerable<Guid>? categoryIds,
            IEnumerable<Guid>? ingredientIds,
            decimal? priceFrom,
            decimal? priceTo)
        {
            IQueryable<Product> query = products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            if (categoryIds is not null && categoryIds.Any())
            {
                List<CategoryId> ids = categoryIds.Select(id => (CategoryId)id).ToList();
                query = query.Where(p => p.Categories.Any(c => ids.Contains(c.Id)));
            }

            if (ingredientIds is not null && ingredientIds.Any())
            {
                List<IngredientId> ids = ingredientIds.Select(id => (IngredientId)id).ToList();
                query = query.Where(p =>
                    p.BaseIngredients.Any(i => ids.Contains(i.Id)) ||
                    p.CustomIngredients.Any(i => ids.Contains(i.Id)));
            }

            if (priceFrom.HasValue)
            {
                query = query.Where(p => p.Price >= priceFrom.Value);
            }

            if (priceTo.HasValue)
            {
                query = query.Where(p => p.Price <= priceTo.Value);
            }

            return query;
        }

        public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
            => await products.Exists(p => p.Name.ToLower() != name.ToLower(), cancellationToken);

        public async Task<bool> IsProductNameUniqueExcludingAsync(string name, ProductId excludeId, CancellationToken cancellationToken = default)
            => !await products.Exists(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);

        public async Task<bool> IsCategoryUsedByProductAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
            => await products.Exists(p => p.Categories.Any(c => c.Id == categoryId), cancellationToken);

        public async Task<bool> IsIngredientUsedAsBaseByProductAsync(IngredientId ingredientId, CancellationToken cancellationToken = default)
            => await products.Exists(p => p.BaseIngredients.Any(i => i.Id == ingredientId), cancellationToken);

        public async Task<bool> IsIngredientUsedAsCustomByProductAsync(IngredientId ingredientId, CancellationToken cancellationToken = default)
            => await products.Exists(p => p.CustomIngredients.Any(i => i.Id == ingredientId), cancellationToken);
    }
}
