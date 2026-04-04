using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;

internal static class Mappings
{
    public static GetCategoriesByTypeResponse ToResponse(this Category category)
    {
        return new GetCategoriesByTypeResponse(category.Id.Value, category.Name, category.SortOrder);
    }
}
