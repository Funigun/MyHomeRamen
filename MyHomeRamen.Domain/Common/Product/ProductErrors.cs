namespace MyHomeRamen.Domain.Common.Product;

public static class ProductErrors
{
    public static DomainException NameTooShort()
        => new($"Product name is too short. Minimum length is {ProductConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Product name exceeds maximum length of {ProductConstants.MaxNameLength}");

    public static DomainException DescriptionTooShort()
    => new($"Product description is too short. Minimum length is {ProductConstants.MinDescriptionLength}");

    public static DomainException DescriptionTooLong()
        => new($"Product description exceeds maximum length of {ProductConstants.MaxDescriptionLength}");

    public static DomainException PriceTooSmall()
        => new($"Product price can not be smaller than {ProductConstants.MinPrice}");

    public static DomainException PriceTooHigh()
        => new($"Product price can not be greater than {ProductConstants.MaxPrice}");

    public static DomainException IngredientsNotUnique()
        => new("Product ingredients must be unique.");

    public static DomainException CustomIngredientsNotUnique()
    => new("Product custom ingredients must be unique.");

    public static DomainException IngredientsOverlapAcrossCollections()
        => new("Product base and custom ingredients must be unique across both collections.");

    public static DomainException CategoryTypeNotValid()
        => new("Product must belong to product categories group.");

    public static DomainException CategoriesNotUnique()
    => new("Product categories must be unique.");

    public static DomainException CategoryRequired()
        => new("Product must have at least one category.");
}
