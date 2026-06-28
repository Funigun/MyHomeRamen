using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

internal static class Mappings
{
    public static GetCategoriesByTypeResponse ToResponse(this Category category)
    {
        return new GetCategoriesByTypeResponse(category.Id.Value, category.Name, category.SortOrder);
    }
}
