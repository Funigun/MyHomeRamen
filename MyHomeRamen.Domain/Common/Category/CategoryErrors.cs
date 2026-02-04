namespace MyHomeRamen.Domain.Common.Category;

public static class CategoryErrors
{
    public static DomainException NameTooShort()
        => new($"Category name is too short. Minimum length is {CategoryConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Category name exceeds maximum length of {CategoryConstants.MaxNameLength}");

    public static DomainException SortOrderTooSmall()
        => new($"Category sort order cannot be negative. Minimum value is {CategoryConstants.MinSortOrder}");

    public static DomainException CategoryTypeInvalid()
        => new("Category type is invalid.");
}
