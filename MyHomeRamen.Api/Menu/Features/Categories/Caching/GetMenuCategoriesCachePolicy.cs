using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.Caching;

internal sealed class GetMenuCategoriesCachePolicy : ICachePolicy<GetMenuCategoriesRequest, List<GetMenuCategoriesResponse>>
{
    public string GetKey(GetMenuCategoriesRequest request) => CacheKeyProvider.GetMenuCategoriesKey();

    public TimeSpan? LocalExpirationTime => TimeSpan.FromSeconds(30);

    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(60);

    public IEnumerable<string> Tags => ["categories"];
}
