using FluentValidation.Results;
using MyHomeRamen.Common.Contracts.Menu.Categories;
using MyHomeRamen.Domain.Common.Category;

namespace MyHomeRamen.UnitTests.MenuModule.Categories;

public sealed class CategoryValidatorsTests
{
    [Fact]
    public void CategoryNameValidator_Should_HaveSameMinLengthAsDomain()
    {
        int domainValue = CategoryConstants.MinNameLength;
        int validatorValue = CategoryNameValidator.MinLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void CategoryNameValidator_Should_HaveSameMaxLengthAsDomain()
    {
        int domainValue = CategoryConstants.MaxNameLength;
        int validatorValue = CategoryNameValidator.MaxLength;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void CategoryNameValidator_Should_Fail_When_NameIsEmpty()
    {
        // Arrange
        CategoryNameValidator validator = new();
        string name = string.Empty;

        // Act
        ValidationResult result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("not empty", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CategoryNameValidator_Should_Fail_When_NameIsTooShort()
    {
        // Arrange
        CategoryNameValidator validator = new();
        string name = new('a', CategoryNameValidator.MinLength - 1);

        // Act
        ValidationResult result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("minimum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CategoryNameValidator_Should_Fail_When_NameIsTooLong()
    {
        // Arrange
        CategoryNameValidator validator = new();
        string name = new('a', CategoryNameValidator.MaxLength + 1);

        // Act
        ValidationResult result = validator.Validate(name);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("maximum length", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CategoryNameValidator_Should_Pass_When_NameIsValid()
    {
        // Arrange
        CategoryNameValidator validator = new();
        string name = "Soups";

        // Act
        ValidationResult result = validator.Validate(name);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CategorySortOrderValidator_Should_HaveSameMinSortOrderAsDomain()
    {
        int domainValue = CategoryConstants.MinSortOrder;
        int validatorValue = CategorySortOrderValidator.MinSortOrder;

        Assert.True(domainValue == validatorValue);
    }

    [Fact]
    public void CategorySortOrderValidator_Should_Fail_When_SortOrderIsBelowMinimum()
    {
        // Arrange
        CategorySortOrderValidator validator = new();
        int sortOrder = CategorySortOrderValidator.MinSortOrder - 1;

        // Act
        ValidationResult result = validator.Validate(sortOrder);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => string.IsNullOrEmpty(e.PropertyName) && e.ErrorMessage.Contains("greater than or equal to", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CategorySortOrderValidator_Should_Pass_When_SortOrderIsValid()
    {
        // Arrange
        CategorySortOrderValidator validator = new();
        int sortOrder = CategorySortOrderValidator.MinSortOrder;

        // Act
        ValidationResult result = validator.Validate(sortOrder);

        // Assert
        Assert.True(result.IsValid);
    }
}
