using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Category> categories)
    {
        public IQueryable<Category> ForCategoryType(CategoryType categoryType)
            => categories.AsNoTracking()
                         .Where(c => c.CategoryType == categoryType)
                         .OrderBy(c => c.SortOrder);

        public async Task<bool> IsCategoryNameUniqueAsync(string name, CancellationToken cancellationToken = default)
            => !await categories.AsNoTracking().AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);

        public async Task<int> GetNextSortOrderAsync(CategoryType categoryType, CancellationToken cancellationToken = default)
        {
            bool any = await categories.AnyAsync(c => c.CategoryType == categoryType, cancellationToken);

            if (!any)
            {
                return CategoryConstants.MinSortOrder;
            }

            return await categories
                .Where(c => c.CategoryType == categoryType)
                .MaxAsync(c => c.SortOrder, cancellationToken) + 1;
        }

        public async Task<List<Category>> GetRemainingForResequencingAsync
        (
            CategoryType categoryType,
            CategoryId excludeId,
            CancellationToken cancellationToken = default
        )
        => await categories.Where(c => c.CategoryType == categoryType && c.Id != excludeId)
                           .OrderBy(c => c.SortOrder)
                           .ToListAsync(cancellationToken);

        public async Task<bool> CategoryExistsAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
            => await categories.AsNoTracking().AnyAsync(c => c.Id == categoryId, cancellationToken);

        public async Task<bool> IsProductCategoryTypeAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
            => await categories.AsNoTracking().AnyAsync(c => c.Id == categoryId && c.CategoryType == CategoryType.Product, cancellationToken);
    }
}
