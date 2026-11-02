using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.UnitTests.MenuModule.Ingredients;

public class IngredientValidationTests
{
    private static readonly IngredientId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultCompanyId = Guid.CreateVersion7();
    private const string DefaultName = "Fresh Garlic";
    private const string DefaultDescription = "Organic fresh garlic.";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Collection<Category> categories = [];

        // Act
        Ingredient ingredient = Ingredient.Create(DefaultId, DefaultCompanyId, DefaultName, DefaultDescription, categories);

        // Assert
        Assert.Equal(DefaultId, ingredient.Id);
        Assert.Equal(DefaultName, ingredient.Name);
        Assert.Equal(DefaultDescription, ingredient.Description);
        Assert.Equal(DefaultCompanyId, ingredient.CompanyId);
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
    public void Create_Should_ThrowDomainException_When_CategoryTypeIsNotValid()
    {
        // Arrange
        // CategoryType.Product (1) is invalid for ingredients
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultCompanyId, "ProdCat", 1, CategoryType.Product);
        Collection<Category> categories = [category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(categories: categories));
        Assert.Equal(IngredientErrors.CategoryTypeNotValid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoriesAreNotUnique()
    {
        // Arrange
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultCompanyId, "IngCat", 1, CategoryType.Ingredient);
        Collection<Category> categories = [category, category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateIngredient(categories: categories));
        Assert.Equal(IngredientErrors.CategoriesNotUnique().Message, exception.Message);
    }

    private static Ingredient CreateIngredient(
        string? name = null,
        string? description = null,
        Collection<Category>? categories = null)
    {
        return Ingredient.Create(
            DefaultId,
            DefaultCompanyId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            categories ?? []);
    }
}
