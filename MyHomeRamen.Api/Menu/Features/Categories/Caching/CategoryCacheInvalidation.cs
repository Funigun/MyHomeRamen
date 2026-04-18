using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.Caching;

internal static class CategoryCacheInvalidation
{
    public static IEnumerable<string> GetAffectedKeys(Category category)
    {
        yield return CacheKeyProvider.GetCategoriesByTypeKey(category.CategoryType.ToString());
        yield return CacheKeyProvider.GetMenuItemsByCategoryKey(category.Id.ToString());
    }

    public static IEnumerable<string> GetAffectedKeys(IEnumerable<Category> categories)
    {
        List<string> keys = [];
        HashSet<CategoryType> affectedCategoryTypes = [];

        foreach (Category category in categories)
        {
            if (!affectedCategoryTypes.Contains(category.CategoryType))
            {
                keys.Add(CacheKeyProvider.GetCategoriesByTypeKey(category.CategoryType.ToString()));
            }

            keys.Add(CacheKeyProvider.GetMenuItemsByCategoryKey(category.Id.ToString()));
        }

        return keys;
    }
}
