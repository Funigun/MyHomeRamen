using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.UnitTests.MenuModule.Ingredients;

public sealed class IngredientUpdateTests
{
    private static readonly IngredientId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Fresh Garlic";
    private const string DefaultDescription = "Organic fresh garlic.";
    private const decimal DefaultPrice = 5.0m;

    private static readonly Category DefaultCategory =
        Category.Create(new CategoryId(Guid.NewGuid()), "Vegetables", 1, CategoryType.Ingredient);

    [Fact]
    public void Update_Should_UpdateProperties_When_InputIsValid()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        Category updatedCategory = Category.Create(new CategoryId(Guid.NewGuid()), "Spices", 2, CategoryType.Ingredient);
        string newName = "Updated Garlic Name";
        string newDescription = "Updated description text.";
        decimal newPrice = 8.5m;

        // Act
        ingredient.Update(newName, newDescription, newPrice, [updatedCategory]);

        // Assert
        Assert.Equal(newName, ingredient.Name);
        Assert.Equal(newDescription, ingredient.Description);
        Assert.Equal(newPrice, ingredient.Price);
        Assert.Single(ingredient.Categories);
        Assert.Equal(updatedCategory.Id, ingredient.Categories[0].Id);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        string name = new('a', IngredientConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, name: name));
        Assert.Equal(IngredientErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        string name = new('a', IngredientConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, name: name));
        Assert.Equal(IngredientErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_DescriptionIsTooLong()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        string description = new('a', IngredientConstants.MaxDescriptionLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, description: description));
        Assert.Equal(IngredientErrors.DescriptionTooLong().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_PriceIsBelowMinimum()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        decimal price = IngredientConstants.MinPrice - 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, price: price));
        Assert.Equal(IngredientErrors.PriceTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_PriceIsAboveMaximum()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        decimal price = IngredientConstants.MaxPrice + 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, price: price));
        Assert.Equal(IngredientErrors.PriceTooHigh().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_CategoriesContainWrongType()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        Category productCategory = Category.Create(new CategoryId(Guid.NewGuid()), "ProdCat", 1, CategoryType.Product);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateIngredient(ingredient, categories: [productCategory]));
        Assert.Equal(IngredientErrors.CategoryTypeNotValid().Message, exception.Message);
    }

    private static Ingredient CreateIngredient()
    {
        return Ingredient.Create(DefaultId, DefaultName, DefaultDescription, DefaultPrice, [DefaultCategory]);
    }

    private static void UpdateIngredient(
        Ingredient ingredient,
        string? name = null,
        string? description = null,
        decimal? price = null,
        Collection<Category>? categories = null)
    {
        ingredient.Update(
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            categories ?? [DefaultCategory]);
    }
}
