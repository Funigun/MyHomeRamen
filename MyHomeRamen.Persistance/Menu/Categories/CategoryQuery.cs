using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategoryQuery
{
    private IQueryable<Category> CategoriesQuery => Categories.AsNoTracking();

    public async Task<List<Category>> GetByType(CategoryType categoryType, CancellationToken cancellationToken)
        => await CategoriesQuery.Where(c => c.CategoryType == categoryType)
                                .OrderBy(c => c.SortOrder)
                                .ToListAsync(cancellationToken);

    public async Task<int> GetNextSortOrder(CategoryType categoryType, CancellationToken cancellationToken)
    {
        bool any = await CategoriesQuery.AnyAsync(c => c.CategoryType == categoryType, cancellationToken);

        if (!any)
        {
            return CategoryConstants.MinSortOrder;
        }

        return await CategoriesQuery.Where(c => c.CategoryType == categoryType)
                                    .MaxAsync(c => c.SortOrder, cancellationToken) + 1;
    }

    public async Task<IEnumerable<Category>> GetByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken)
        => await CategoriesQuery.Where(c => categoryIds.Contains(c.Id)).ToListAsync(cancellationToken);

    public async Task<bool> Exists(CategoryId categoryId, CancellationToken cancellationToken)
        => await CategoriesQuery.AnyAsync(c => c.Id == categoryId, cancellationToken);

    public async Task<bool> IsCategoryNameUnique(string name, CancellationToken cancellationToken)
        => !await CategoriesQuery.Exists(c => c.Name == name, cancellationToken);

    public async Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken)
        => await CategoriesQuery.Exists(c => c.Id == categoryId && c.CategoryType == CategoryType.Product, cancellationToken);

    public async Task<bool> IsUsedByProducts(CategoryId categoryId, CancellationToken cancellationToken)
        => await Product.Exists(product => product.Categories.Any(category => category.Id == categoryId), cancellationToken);

    public async Task<bool> IsUsedByIngredients(CategoryId categoryId, CancellationToken cancellationToken)
        => await Ingredients.Exists(ingredient => ingredient.Categories.Any(category => category.Id == categoryId), cancellationToken);
}
