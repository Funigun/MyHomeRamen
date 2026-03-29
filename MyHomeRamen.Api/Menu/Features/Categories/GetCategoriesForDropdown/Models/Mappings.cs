using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;

internal static class Mappings
{
    public static GetCategoriesForDropdownResponse ToResponse(this Category category)
    {
        return new GetCategoriesForDropdownResponse(category.Id.Value, category.Name);
    }
}
