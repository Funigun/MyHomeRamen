using FluentValidation.Results;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Validators;
using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketItemCommentValidatorTests
{
    private readonly BasketItemCommentValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommentIsWithinLimit()
    {
        // Arrange
        string comment = new('a', BasketItemCommentValidator.MaxCommentLength);

        // Act
        ValidationResult result = _validator.Validate(comment);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenCommentExceedsMaxLength()
    {
        // Arrange
        string comment = new('a', BasketItemCommentValidator.MaxCommentLength + 1);

        // Act
        ValidationResult result = _validator.Validate(comment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("not exceed"));
    }

    [Fact]
    public void MaxCommentLength_ShouldMatch_DomainConstant()
    {
        // Assert
        Assert.Equal(BasketConstants.MaxCommentLength, BasketItemCommentValidator.MaxCommentLength);
    }
}
