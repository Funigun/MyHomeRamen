using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Category> categories)
    {
        public IQueryable<Category> ForCategoryType(CategoryType categoryType)
            => categories.GetListQuery(
                orderBy: c => c.SortOrder,
                filter: c => c.CategoryType == categoryType);

        public async Task<bool> IsCategoryNameUniqueAsync(string name, CancellationToken cancellationToken = default)
            => await categories.Exists(c => c.Name.ToLower() != name.ToLower(), cancellationToken);

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
            => await categories.Exists(c => c.Id == categoryId, cancellationToken);

        public async Task<bool> IsProductCategoryTypeAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
            => await categories.Exists(c => c.Id == categoryId && c.CategoryType == CategoryType.Product, cancellationToken);
    }
}
