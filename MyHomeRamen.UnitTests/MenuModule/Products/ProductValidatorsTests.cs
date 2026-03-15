using MyHomeRamen.Common.Contracts.Menu.Products;

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
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("not empty", StringComparison.OrdinalIgnoreCase));
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
    public void ProductNameValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = Domain.Common.Product.ProductConstants.MaxNameLength;
        int validatorValue = ProductNameValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }
}
