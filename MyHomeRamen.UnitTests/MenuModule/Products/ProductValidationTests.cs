using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.UnitTests.MenuModule.Products;

public class ProductValidationTests
{
    private const string DefaultName = "Delicious Ramen Soup";
    private const string DefaultDescription = "This is a very delicious ramen soup that everyone loves to eat.";
    private const decimal DefaultPrice = 50.0m;
    private const string DefaultImageUrl = "http://example.com/ramen.jpg";

    private static readonly ProductId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Collection<Ingredient> baseIngredients = [];
        Collection<Ingredient> customIngredients = [];
        Collection<Category> categories = [];

        // Act
        Product product = Product.Create(DefaultId, DefaultRestaurantId, DefaultName, DefaultDescription, DefaultPrice, DefaultImageUrl, baseIngredients, customIngredients, categories);

        // Assert
        Assert.Equal(DefaultId, product.Id);
        Assert.Equal(DefaultName, product.Name);
        Assert.Equal(DefaultDescription, product.Description);
        Assert.Equal(DefaultPrice, product.Price);
        Assert.Equal(DefaultImageUrl, product.ImageUrl);
        Assert.Equal(baseIngredients, product.BaseIngredients);
        Assert.Equal(customIngredients, product.CustomIngredients);
        Assert.Equal(categories, product.Categories);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', ProductConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(name: name));
        Assert.Equal(ProductErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', ProductConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(name: name));
        Assert.Equal(ProductErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_DescriptionIsTooShort()
    {
        // Arrange
        string description = new('a', ProductConstants.MinDescriptionLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(description: description));
        Assert.Equal(ProductErrors.DescriptionTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_DescriptionIsTooLong()
    {
        // Arrange
        string description = new('a', ProductConstants.MaxDescriptionLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(description: description));
        Assert.Equal(ProductErrors.DescriptionTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PriceIsTooSmall()
    {
        // Arrange
        decimal price = ProductConstants.MinPrice - 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(price: price));
        Assert.Equal(ProductErrors.PriceTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PriceIsTooHigh()
    {
         // Arrange
        decimal price = ProductConstants.MaxPrice + 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(price: price));
        Assert.Equal(ProductErrors.PriceTooHigh().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_BaseIngredientsAreNotUnique()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        Collection<Ingredient> baseIngredients = [ingredient, ingredient];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(baseIngredients: baseIngredients));
        Assert.Equal(ProductErrors.IngredientsNotUnique().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CustomIngredientsAreNotUnique()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        Collection<Ingredient> customIngredients = [ingredient, ingredient];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(customIngredients: customIngredients));
        Assert.Equal(ProductErrors.CustomIngredientsNotUnique().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoryTypeIsNotValid()
    {
        // Arrange
        // CategoryType.Ingredient (2) is invalid for products
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultRestaurantId, "IngCat", 1, CategoryType.Ingredient);
        Collection<Category> categories = [category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(categories: categories));
        Assert.Equal(ProductErrors.CategoryTypeNotValid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CategoriesAreNotUnique()
    {
        // Arrange
        Category category = Category.Create(new CategoryId(Guid.NewGuid()), DefaultRestaurantId, "ProdCat", 1, CategoryType.Product);
        Collection<Category> categories = [category, category];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(categories: categories));
        Assert.Equal(ProductErrors.CategoriesNotUnique().Message, exception.Message);
    }

    private static Product CreateProduct(
        string? name = null,
        string? description = null,
        decimal? price = null,
        Collection<Ingredient>? baseIngredients = null,
        Collection<Ingredient>? customIngredients = null,
        Collection<Category>? categories = null)
    {
        return Product.Create(
            DefaultId,
            DefaultRestaurantId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            DefaultImageUrl,
            baseIngredients ?? [],
            customIngredients ?? [],
            categories ?? []);
    }

    private static Ingredient CreateIngredient()
    {
        return Ingredient.Create(
            new IngredientId(Guid.NewGuid()),
            DefaultRestaurantId,
            "Ingredient",
            "Description",
            10.0m,
            []);
    }
}
