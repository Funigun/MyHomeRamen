using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;

internal static class Mappings
{
    public static CategoryForManageDto ToManageDto(this Category category)
    {
        return new CategoryForManageDto(category.Id.Value, category.Name, category.SortOrder);
    }
}
