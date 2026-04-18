using System.Text;

namespace MyHomeRamen.Api.Common.Cache;

public static class CacheKeyProvider
{
    public static readonly CompositeFormat CategoryByTypeKeyTemplate = CompositeFormat.Parse("categories_by_type_{0}");

    public static readonly CompositeFormat MenuItemsByCategoryKeyTemplate = CompositeFormat.Parse("products_by_category_{0}");

    public static string GetCategoriesByTypeKey(string categoryType) => string.Format(null, CategoryByTypeKeyTemplate, categoryType);

    public static string GetMenuItemsByCategoryKey(string categoryId) => string.Format(null, MenuItemsByCategoryKeyTemplate, categoryId);
}
