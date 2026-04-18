using MyHomeRamen.Api.Common.Cache;

namespace MyHomeRamen.Api.Menu.Features.Products.Caching;

internal static class ProductCacheInvalidation
{
    public static IEnumerable<string> GetAffectedKeys(Guid categoryId)
    {
        yield return CacheKeyProvider.GetMenuItemsByCategoryKey(categoryId.ToString());
    }

    public static IEnumerable<string> GetAffectedKeys(IEnumerable<Guid> categoryIds)
    {
        return categoryIds.SelectMany(type => GetAffectedKeys(type));
    }
}
