using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Policies;

public class GetProductsByCategoryCachePolicy : ICachePolicy<GetProductsByCategoryRequest, List<GetProductsByCategoryResponse>>
{
    public TimeSpan? ExpirationTime { get; } = TimeSpan.FromMinutes(60);

    public TimeSpan? LocalExpirationTime { get; } = TimeSpan.FromMinutes(30);

    public string GetKey(GetProductsByCategoryRequest request)
    {
        return $"products_by_category_{request.CategoryId}";
    }
}
