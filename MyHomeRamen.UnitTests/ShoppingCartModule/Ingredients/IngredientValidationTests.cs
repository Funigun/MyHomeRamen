using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Ingredients;

public class IngredientValidationTests
{
    private static readonly IngredientId DefaultId = new(Guid.NewGuid());
    private static readonly IngredientId DefaultOriginalId = new(Guid.NewGuid());
    private const string DefaultName = "Fresh Garlic";
    private const string DefaultDescription = "Organic fresh garlic.";
    private const decimal DefaultPrice = 5.0m;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Ingredient ingredient = Ingredient.Create(DefaultId, DefaultOriginalId, DefaultName, DefaultDescription, DefaultPrice);

        // Assert
        Assert.Equal(DefaultId, ingredient.Id);
        Assert.Equal(DefaultOriginalId, ingredient.OriginalId);
        Assert.Equal(DefaultName, ingredient.Name);
        Assert.Equal(DefaultDescription, ingredient.Description);
        Assert.Equal(DefaultPrice, ingredient.Price);
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
    public void Create_Should_ThrowDomainException_When_DescriptionIsTooShort()
    {
        // Arrange
        string description = new('a', IngredientConstants.MinDescriptionLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(description: description));
        Assert.Equal(IngredientErrors.DescriptionTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_DescriptionIsTooLong()
    {
        // Arrange
        string description = new('a', IngredientConstants.MaxDescriptionLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(description: description));
        Assert.Equal(IngredientErrors.DescriptionTooLong().Message, exception.Message);
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
        string? description = null,
        decimal? price = null)
    {
        return Ingredient.Create(
            DefaultId,
            DefaultOriginalId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice);
    }
}
