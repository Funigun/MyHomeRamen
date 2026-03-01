using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Orders.Ingredients;

namespace MyHomeRamen.UnitTests.OrdersModule.Ingredients;

public class IngredientValidationTests
{
    private static readonly IngredientId DefaultId = new(Guid.NewGuid());
    private static readonly IngredientId DefaultOriginalId = new(Guid.NewGuid());
    private const string DefaultName = "Fresh Garlic";
    private const decimal DefaultPrice = 5.0m;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Ingredient ingredient = Ingredient.Create(DefaultId, DefaultOriginalId, DefaultName, DefaultPrice);

        // Assert
        Assert.Equal(DefaultId, ingredient.Id);
        Assert.Equal(DefaultOriginalId, ingredient.OriginalId);
        Assert.Equal(DefaultName, ingredient.Name);
        Assert.Equal(DefaultPrice, ingredient.OriginalPrice);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', IngredientConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(name: name));
        Assert.Equal(IngredientErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', IngredientConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(name: name));
        Assert.Equal(IngredientErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PriceIsTooSmall()
    {
        // Arrange
        decimal price = IngredientConstants.MinPrice - 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(price: price));
        Assert.Equal(IngredientErrors.PriceTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PriceIsTooHigh()
    {
        // Arrange
        decimal price = IngredientConstants.MaxPrice + 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(price: price));
        Assert.Equal(IngredientErrors.PriceTooHigh().Message, exception.Message);
    }

    private static Ingredient CreateIngredient(
        string? name = null,
        decimal? price = null)
    {
        return Ingredient.Create(
            DefaultId,
            DefaultOriginalId,
            name ?? DefaultName,
            price ?? DefaultPrice);
    }
}
