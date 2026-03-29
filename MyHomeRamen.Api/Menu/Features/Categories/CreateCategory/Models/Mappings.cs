using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;

internal static class Mappings
{
    public static Category ToDomain(this CreateCategoryRequest request, int nextSortOrder)
    {
        return Category.Create(
            Guid.NewGuid(),
            request.Name,
            nextSortOrder,
            (CategoryType)request.CategoryType);
    }
}
