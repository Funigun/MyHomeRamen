using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class CategoryRepository : ICategoryQuery
{
    public async Task<IEnumerable<Category>> GetByType(CategoryType categoryType, CancellationToken cancellationToken)
        => await QueryList(
            DbQueryOptions<Category>.Where(c => c.CategoryType == categoryType).OrderByAsc(c => c.SortOrder),
            c => c,
            cancellationToken);

    public async Task<int> GetNextSortOrder(CategoryType categoryType, CancellationToken cancellationToken)
    {
        bool any = await Exists(c => c.CategoryType == categoryType, cancellationToken);

        if (!any)
        {
            return CategoryConstants.MinSortOrder;
        }

        return await menuDbContext.Categories.AsNoTracking()
                                             .Where(c => c.CategoryType == categoryType)
                                             .MaxAsync(c => c.SortOrder, cancellationToken) + 1;
    }

    public async Task<IEnumerable<Category>> GetByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken)
        => await QueryList(DbQueryOptions<Category>.Where(c => categoryIds.Contains(c.Id)), c => c, cancellationToken);

    public async Task<bool> IsCategoryNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(c => c.Name == name, cancellationToken);

    public async Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken)
        => await Exists(c => c.Id == categoryId && c.CategoryType == CategoryType.Product, cancellationToken);

    public async Task<bool> IsUsedByProducts(CategoryId categoryId, CancellationToken cancellationToken)
        => await menuDbContext.Product.Exists(product => product.Categories.Any(category => category.Id == categoryId), cancellationToken);

    public async Task<bool> IsUsedByIngredients(CategoryId categoryId, CancellationToken cancellationToken)
        => await menuDbContext.Ingredient.Exists(ingredient => ingredient.Categories.Any(category => category.Id == categoryId), cancellationToken);

    public async Task<Category?> ById(CategoryId id, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(c => c.Id == id, c => c, cancellationToken);
}
