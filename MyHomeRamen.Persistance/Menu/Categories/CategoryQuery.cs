using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;
using MyHomeRamen.Persistance.Cache;

namespace MyHomeRamen.Persistance.Menu;

public partial class CategoryRepository : ICategoryQuery
{
    public async Task<IEnumerable<CategoryByTypeDto>> GetByTypeDto(GetCategoryByTypeQueryOptions options, CancellationToken cancellationToken)
    {
        string cacheKey = $"CategoryByTypeDto:{options.CategoryType}";
        TimeSpan cacheExpirationTime = TimeSpan.FromMinutes(5, 30);
        IEnumerable<string> cacheTags = [$"categories:{options.CategoryType}"];

        CachePolicy policy = CachePolicy.LocalCache<MenuCacheModule>(cacheKey, cacheExpirationTime, cacheTags);

        return await QueryList(menuDbContext.Categories, options, policy, cancellationToken);
    }

    public async Task<IEnumerable<CategoryForMenuDto>> GetMenuCategories(GetMenuCategoriesQueryOptions options, CancellationToken cancellationToken)
    {
        return await QueryList(menuDbContext.Categories, options, cancellationToken);
    }

    public async Task<int> GetNextSortOrder(CategoryType categoryType, CancellationToken cancellationToken)
    {
        bool any = await Exists(c => c.CategoryType == categoryType, cancellationToken);

        return any ? await menuDbContext.Categories.AsNoTracking()
                                             .Where(c => c.CategoryType == categoryType)
                                             .MaxAsync(c => c.SortOrder, cancellationToken) + 1 
                   : CategoryConstants.MinSortOrder;
    }

    public async Task<IEnumerable<Category>> GetByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken)
    {
        DbQueryOptions<Category, Category> options = new() 
        { 
            Filter = c => categoryIds.Contains(c.Id), 
            Selector = c => Category.Create(c.Id, c.Name, c.SortOrder, c.CategoryType)
        };

        return await QueryList(menuDbContext.Categories, options, cancellationToken);
    }

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
