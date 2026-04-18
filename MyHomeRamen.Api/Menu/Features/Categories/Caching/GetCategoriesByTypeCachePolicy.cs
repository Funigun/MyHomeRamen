using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.Caching;

internal sealed class GetCategoriesByTypeCachePolicy : ICachePolicy<GetCategoriesByTypeRequest, List<GetCategoriesByTypeResponse>>
{
    public string GetKey(GetCategoriesByTypeRequest request) => CacheKeyProvider.GetCategoriesByTypeKey(request.CategoryType.ToString());

    public TimeSpan? LocalExpirationTime => TimeSpan.FromSeconds(30);

    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(60);

    public IEnumerable<string> Tags => ["categories"];
}
