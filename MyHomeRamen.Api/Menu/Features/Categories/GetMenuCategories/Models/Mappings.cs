using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;

internal static class Mappings
{
    public static GetMenuCategoriesResponse ToMenuResponse(this Category category)
    {
        return new GetMenuCategoriesResponse(category.Id.Value, category.Name);
    }
}
