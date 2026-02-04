using MyHomeRamen.Domain.Common.Category;

namespace MyHomeRamen.Domain.Menu.Categories;

internal static class CategoryValidator
{
    internal static void Validate(Category category)
    {
        CheckName(category);
        CheckSortOrder(category);
        CheckCategoryType(category);
    }

    private static void CheckName(Category category)
    {
        if (category.Name.Length < CategoryConstants.MinNameLength)
        {
            throw CategoryErrors.NameTooShort();
        }

        if (category.Name.Length > CategoryConstants.MaxNameLength)
        {
            throw CategoryErrors.NameTooLong();
        }
    }

    private static void CheckSortOrder(Category category)
    {
        if (category.SortOrder < CategoryConstants.MinSortOrder)
        {
            throw CategoryErrors.SortOrderTooSmall();
        }
    }

    private static void CheckCategoryType(Category category)
    {
        if (!Enum.IsDefined(category.CategoryType))
        {
            throw CategoryErrors.CategoryTypeInvalid();
        }
    }
}
