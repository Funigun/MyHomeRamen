using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Policies;

internal sealed class GetCategoriesByTypeCachePolicy : ICachePolicy<GetCategoriesByTypeRequest, List<GetCategoriesByTypeResponse>>
{
    public string GetKey(GetCategoriesByTypeRequest request) => $"categories_by_type_{request.CategoryType}";

    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(60);

    public TimeSpan? LocalExpirationTime => TimeSpan.FromSeconds(30);
}
