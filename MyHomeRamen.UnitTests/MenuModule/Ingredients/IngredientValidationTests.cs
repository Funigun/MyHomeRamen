using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.UnitTests.MenuModule.Ingredients;

public class IngredientValidationTests
{
    private static readonly IngredientId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();
    private const string DefaultName = "Fresh Garlic";
    private const string DefaultDescription = "Organic fresh garlic.";
    private const decimal DefaultPrice = 5.0m;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Collection<Category> categories = [];

        // Act
        Ingredient ingredient = Ingredient.Create(DefaultId, DefaultRestaurantId, DefaultName, DefaultDescription, DefaultPrice, categories);

        // Assert
        Assert.Equal(DefaultId, ingredient.Id);
        Assert.Equal(DefaultName, ingredient.Name);
        Assert.Equal(DefaultDescription, ingredient.Description);
        Assert.Equal(DefaultPrice, ingredient.Price);
        Assert.Equal(categories, ingredient.Categories);
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

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoryTypeIsNotValid()
    {
        // Arrange
        // CategoryType.Product (1) is invalid for ingredients
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultRestaurantId, "ProdCat", 1, CategoryType.Product);
        Collection<Category> categories = [category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(categories: categories));
        Assert.Equal(IngredientErrors.CategoryTypeNotValid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoriesAreNotUnique()
    {
        // Arrange
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultRestaurantId, "IngCat", 1, CategoryType.Ingredient);
        Collection<Category> categories = [category, category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(categories: categories));
        Assert.Equal(IngredientErrors.CategoriesNotUnique().Message, exception.Message);
    }

    private static Ingredient CreateIngredient(
        string? name = null,
        string? description = null,
        decimal? price = null,
        Collection<Category>? categories = null)
    {
        return Ingredient.Create(
            DefaultId,
            DefaultRestaurantId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            categories ?? []);
    }
}
