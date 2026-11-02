namespace MyHomeRamen.Domain.Common.Ingredient;

public static class IngredientErrors
{
    public static DomainException CompanyRequired()
        => new("Ingredient company identifier must not be empty.");

    public static DomainException CategoryCompanyMismatch()
        => new("Ingredient and categories must belong to the same company.");

    public static DomainException NameTooShort()
        => new($"Ingredient name is too short. Minimum length is {IngredientConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Ingredient name exceeds maximum length of {IngredientConstants.MaxNameLength}");

    public static DomainException DescriptionTooShort()
        => new($"Ingredient description is too short. Minimum length is {IngredientConstants.MinDescriptionLength}");

    public static DomainException DescriptionTooLong()
        => new($"Ingredient description exceeds maximum length of {IngredientConstants.MaxDescriptionLength}");

    public static DomainException PriceTooSmall()
        => new("Ingredient price can not be negative");

    public static DomainException PriceTooHigh()
        => new($"Ingredient price can not be greater than {IngredientConstants.MaxPrice}");

    public static DomainException CategoryTypeNotValid()
        => new("Ingredient must belong to ingredient categories group.");

    public static DomainException CategoriesNotUnique()
        => new("Ingredient categories must be unique.");
}
