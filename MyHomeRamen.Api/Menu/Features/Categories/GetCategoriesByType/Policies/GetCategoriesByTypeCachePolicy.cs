using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Policies;

internal sealed class GetCategoriesByTypeCachePolicy : ICachePolicy<GetCategoriesByTypeRequest, List<Category>>
{
    public string GetKey(GetCategoriesByTypeRequest request) => $"categories_by_type_{request.CategoryType}";

    public TimeSpan? ExpirationTime => null;

    public TimeSpan? LocalExpirationTime => null;
}
