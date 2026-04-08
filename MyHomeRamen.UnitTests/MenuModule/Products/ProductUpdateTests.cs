using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.UnitTests.MenuModule.Products;

public sealed class ProductUpdateTests
{
    private static readonly ProductId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Delicious Ramen Soup";
    private const string DefaultDescription = "This is a very delicious ramen soup that everyone loves to eat.";
    private const decimal DefaultPrice = 50.0m;
    private const string DefaultImageUrl = "http://example.com/ramen.jpg";

    private static readonly Category DefaultCategory =
        Category.Create(new CategoryId(Guid.NewGuid()), "Product Category Name", 1, CategoryType.Product);

    private static readonly Ingredient DefaultIngredient =
        Ingredient.Create(new IngredientId(Guid.NewGuid()), "Valid Ingredient Name", "Valid ingredient description here.", 5.0m, []);

    [Fact]
    public void Update_Should_UpdateProperties_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        Category updatedCategory = Category.Create(new CategoryId(Guid.NewGuid()), "Updated Category Name", 2, CategoryType.Product);
        Ingredient updatedIngredient = Ingredient.Create(new IngredientId(Guid.NewGuid()), "Updated Ingredient Name", "Updated ingredient description here.", 3.0m, []);
        string newName = "Updated Ramen Bowl Soup";
        string newDescription = "This is an updated description for the delicious ramen soup bowl.";
        decimal newPrice = 75.0m;

        // Act
        product.Update(newName, newDescription, newPrice, updatedCategory, [updatedIngredient]);

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.Equal(newDescription, product.Description);
        Assert.Equal(newPrice, product.Price);
        Assert.Single(product.Categories);
        Assert.Equal(updatedCategory.Id, product.Categories[0].Id);
        Assert.Single(product.BaseIngredients);
        Assert.Equal(updatedIngredient.Id, product.BaseIngredients[0].Id);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_NameIsEmpty()
    {
        // Arrange
        Product product = CreateProduct();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateProduct(product, name: string.Empty));
        Assert.Equal(ProductErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        Product product = CreateProduct();
        string name = new('a', ProductConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateProduct(product, name: name));
        Assert.Equal(ProductErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_PriceIsBelowMinimum()
    {
        // Arrange
        Product product = CreateProduct();
        decimal price = ProductConstants.MinPrice - 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateProduct(product, price: price));
        Assert.Equal(ProductErrors.PriceTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_PriceIsAboveMaximum()
    {
        // Arrange
        Product product = CreateProduct();
        decimal price = ProductConstants.MaxPrice + 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => UpdateProduct(product, price: price));
        Assert.Equal(ProductErrors.PriceTooHigh().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_CategoryIsNull()
    {
        // Arrange
        Product product = CreateProduct();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() =>
            product.Update(DefaultName, DefaultDescription, DefaultPrice, null!, [DefaultIngredient]));
        Assert.Equal(ProductErrors.CategoryRequired().Message, exception.Message);
    }

    private static Product CreateProduct(
        string? name = null,
        string? description = null,
        decimal? price = null,
        Collection<Ingredient>? baseIngredients = null,
        Collection<Category>? categories = null)
    {
        return Product.Create(
            DefaultId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            DefaultImageUrl,
            baseIngredients ?? [DefaultIngredient],
            [],
            categories ?? [DefaultCategory]);
    }

    private static void UpdateProduct(
        Product product,
        string? name = null,
        string? description = null,
        decimal? price = null,
        Category? category = null,
        IEnumerable<Ingredient>? ingredients = null)
    {
        product.Update(
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            category ?? DefaultCategory,
            ingredients ?? [DefaultIngredient]);
    }
}
