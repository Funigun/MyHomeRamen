using FluentValidation.Results;
using MyHomeRamen.Common.Contracts.Basket;
using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketItemQuantityValidatorTests
{
    private readonly BasketItemQuantityValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenQuantityIsAtMinimum()
    {
        // Act
        ValidationResult result = _validator.Validate(BasketItemQuantityValidator.MinQuantity);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenQuantityIsAtMaximum()
    {
        // Act
        ValidationResult result = _validator.Validate(BasketItemQuantityValidator.MaxQuantity);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenQuantityIsBelowMinimum()
    {
        // Act
        ValidationResult result = _validator.Validate(BasketItemQuantityValidator.MinQuantity - 1);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("greater than or equal to"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenQuantityIsAboveMaximum()
    {
        // Act
        ValidationResult result = _validator.Validate(BasketItemQuantityValidator.MaxQuantity + 1);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("less than or equal to"));
    }

    [Fact]
    public void MinQuantity_ShouldMatch_DomainConstant()
    {
        // Assert
        Assert.Equal(BasketConstants.MinQuantity, BasketItemQuantityValidator.MinQuantity);
    }

    [Fact]
    public void MaxQuantity_ShouldMatch_DomainConstant()
    {
        // Assert
        Assert.Equal(BasketConstants.MaxQuantity, BasketItemQuantityValidator.MaxQuantity);
    }
}
