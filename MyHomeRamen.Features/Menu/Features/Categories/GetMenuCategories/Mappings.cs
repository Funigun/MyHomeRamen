using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

internal static class Mappings
{
    public static GetMenuCategoriesResponse ToMenuResponse(this Category category)
    {
        return new GetMenuCategoriesResponse(category.Id.Value, category.Name);
    }
}
