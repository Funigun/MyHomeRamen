using MyHomeRamen.Common.Contracts.Menu.Products.Validators;

namespace MyHomeRamen.UnitTests.MenuModule.Products;

public sealed class ProductValidatorsTests
{
    [Fact]
    public void ProductNameValidator_Should_Fail_When_NameIsEmpty()
    {
        // Arrange
        ProductNameValidator? validator = new();
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
    public void ProductNameValidator_Should_Fail_When_NameIsTooLong()
    {
        // Arrange
        ProductNameValidator? validator = new();
        string name = new('a', ProductNameValidator.MaxLength + 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("maximum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductNameValidator_Should_Pass_When_NameIsValid()
    {
        // Arrange
        ProductNameValidator? validator = new();
        string name = "Delicious Ramen Soup";

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProductNameValidator_Should_Fail_When_NameIsTooShort()
    {
        // Arrange
        ProductNameValidator? validator = new();
        string name = new('a', ProductNameValidator.MinLength - 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("minimum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductNameValidator_Should_HaveSameMinLengthAsDomain()
    {
        int domainValue = Domain.Common.Product.ProductConstants.MinNameLength;
        int validatorValue = ProductNameValidator.MinLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void ProductNameValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = Domain.Common.Product.ProductConstants.MaxNameLength;
        int validatorValue = ProductNameValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void ProductDescriptionValidator_Should_Fail_When_DescriptionIsEmpty()
    {
        // Arrange
        ProductDescriptionValidator? validator = new();
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
    public void ProductDescriptionValidator_Should_Fail_When_DescriptionIsTooShort()
    {
        // Arrange
        ProductDescriptionValidator? validator = new();
        string description = new('a', ProductDescriptionValidator.MinLength - 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("minimum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductDescriptionValidator_Should_Fail_When_DescriptionIsTooLong()
    {
        // Arrange
        ProductDescriptionValidator? validator = new();
        string description = new('a', ProductDescriptionValidator.MaxLength + 1);

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("maximum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductDescriptionValidator_Should_Pass_When_DescriptionIsValid()
    {
        // Arrange
        ProductDescriptionValidator? validator = new();
        string description = "A rich and flavorful broth with fresh noodles, tender pork and soft-boiled eggs.";

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(description);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProductDescriptionValidator_Should_HaveSameMinLengthAsDomain()
    {
        int domainValue = Domain.Common.Product.ProductConstants.MinDescriptionLength;
        int validatorValue = ProductDescriptionValidator.MinLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void ProductDescriptionValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = Domain.Common.Product.ProductConstants.MaxDescriptionLength;
        int validatorValue = ProductDescriptionValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void ProductPriceValidator_Should_Fail_When_PriceIsTooLow()
    {
        // Arrange
        ProductPriceValidator? validator = new();
        decimal price = ProductPriceValidator.MinPrice - 0.1m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("less than or equal to", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductPriceValidator_Should_Fail_When_PriceIsTooHigh()
    {
        // Arrange
        ProductPriceValidator? validator = new();
        decimal price = ProductPriceValidator.MaxPrice + 0.1m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("greater than or equal to", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ProductPriceValidator_Should_Pass_When_PriceIsValid()
    {
        // Arrange
        ProductPriceValidator? validator = new();
        decimal price = 10.0m;

        // Act
        FluentValidation.Results.ValidationResult? result = validator.Validate(price);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProductPriceValidator_Should_HaveSameMinPriceAsDomain()
    {
        decimal domainValue = Domain.Common.Product.ProductConstants.MinPrice;
        decimal validatorValue = ProductPriceValidator.MinPrice;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void ProductPriceValidator_Should_HaveSameMaxPriceAsDomain()
    {
        decimal domainValue = Domain.Common.Product.ProductConstants.MaxPrice;
        decimal validatorValue = ProductPriceValidator.MaxPrice;

        Assert.True(domainValue == validatorValue);
    }
}
