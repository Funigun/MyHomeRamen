using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.Caching;

internal class GetProductsByCategoryCachePolicy : ICachePolicy<GetProductsByCategoryRequest, List<GetProductsByCategoryResponse>>
{
    public string GetKey(GetProductsByCategoryRequest request) => CacheKeyProvider.GetMenuItemsByCategoryKey(request.CategoryId.ToString());

    public TimeSpan? LocalExpirationTime { get; } = TimeSpan.FromMinutes(30);

    public TimeSpan? ExpirationTime { get; } = TimeSpan.FromMinutes(60);

    public IEnumerable<string> Tags => ["products"];
}
