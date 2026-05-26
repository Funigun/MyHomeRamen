using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;

namespace MyHomeRamen.UnitTests.MenuModule.Ingredients;

public sealed class IngredientValidatorsTests
{
    [Fact]
    public void IngredientNameValidator_Should_Fail_When_NameIsEmpty()
    {
        // Arrange
        IngredientNameValidator? validator = new();
        string name = string.Empty;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("not be empty", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientNameValidator_Should_Fail_When_NameIsTooShort()
    {
        // Arrange
        IngredientNameValidator? validator = new();
        string name = new('a', IngredientNameValidator.MinLength - 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("minimum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientNameValidator_Should_Fail_When_NameIsTooLong()
    {
        // Arrange
        IngredientNameValidator? validator = new();
        string name = new('a', IngredientNameValidator.MaxLength + 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("maximum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientNameValidator_Should_Pass_When_NameIsValid()
    {
        // Arrange
        IngredientNameValidator? validator = new();
        string name = "Fresh Bamboo Shoots";

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IngredientNameValidator_Should_HaveSameMinLengthAsDomain()
    {
        int domainValue = Domain.Common.Ingredient.IngredientConstants.MinNameLength;
        int validatorValue = IngredientNameValidator.MinLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void IngredientNameValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = Domain.Common.Ingredient.IngredientConstants.MaxNameLength;
        int validatorValue = IngredientNameValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_Fail_When_DescriptionIsEmpty()
    {
        // Arrange
        IngredientDescriptionValidator? validator = new();
        string description = string.Empty;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("not be empty", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_Fail_When_DescriptionIsTooShort()
    {
        // Arrange
        IngredientDescriptionValidator? validator = new();
        string description = new('a', IngredientDescriptionValidator.MinLength - 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("minimum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_Fail_When_DescriptionIsTooLong()
    {
        // Arrange
        IngredientDescriptionValidator? validator = new();
        string description = new('a', IngredientDescriptionValidator.MaxLength + 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("maximum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_Pass_When_DescriptionIsValid()
    {
        // Arrange
        IngredientDescriptionValidator? validator = new();
        string description = "Thinly sliced bamboo shoots";

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_HaveSameMinLengthAsDomain()
    {
        int domainValue = Domain.Common.Ingredient.IngredientConstants.MinDescriptionLength;
        int validatorValue = IngredientDescriptionValidator.MinLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void IngredientDescriptionValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = Domain.Common.Ingredient.IngredientConstants.MaxDescriptionLength;
        int validatorValue = IngredientDescriptionValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void IngredientPriceValidator_Should_Fail_When_PriceIsTooLow()
    {
        // Arrange
        IngredientPriceValidator? validator = new();
        decimal price = IngredientPriceValidator.MinPrice - 0.1m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("less than or equal to", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientPriceValidator_Should_Fail_When_PriceIsTooHigh()
    {
        // Arrange
        IngredientPriceValidator? validator = new();
        decimal price = IngredientPriceValidator.MaxPrice + 0.1m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("greater than or equal to", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IngredientPriceValidator_Should_Pass_When_PriceIsValid()
    {
        // Arrange
        IngredientPriceValidator? validator = new();
        decimal price = 5.0m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IngredientPriceValidator_Should_HaveSameMinPriceAsDomain()
    {
        decimal domainValue = Domain.Common.Ingredient.IngredientConstants.MinPrice;
        decimal validatorValue = IngredientPriceValidator.MinPrice;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void IngredientPriceValidator_Should_HaveSameMaxPriceAsDomain()
    {
        decimal domainValue = Domain.Common.Ingredient.IngredientConstants.MaxPrice;
        decimal validatorValue = IngredientPriceValidator.MaxPrice;

        Assert.True(domainValue == validatorValue);
    }
}
