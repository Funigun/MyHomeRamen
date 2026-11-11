using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

internal static class CategoryCacheTags
{
    public static string ByType(CategoryType categoryType) => $"Menu_categories:{categoryType}";
}
