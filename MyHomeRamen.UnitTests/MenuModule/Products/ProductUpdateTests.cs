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
    private const string DefaultImageUrl = "http://example.com/ramen.jpg";
    private static readonly Guid DefaultCompanyId = Guid.CreateVersion7();

    private static readonly Category DefaultCategory =
        Category.Create(new CategoryId(Guid.NewGuid()), DefaultCompanyId, "Product Category Name", 1, CategoryType.Product);

    private static readonly Ingredient DefaultIngredient =
        Ingredient.Create(new IngredientId(Guid.NewGuid()), DefaultCompanyId, "Valid Ingredient Name", "Valid ingredient description here.", []);

    [Fact]
    public void Update_Should_UpdateProperties_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        Category updatedCategory = Category.Create(new CategoryId(Guid.NewGuid()), DefaultCompanyId, "Updated Category Name", 2, CategoryType.Product);
        Ingredient updatedIngredient = Ingredient.Create(new IngredientId(Guid.NewGuid()), DefaultCompanyId, "Updated Ingredient Name", "Updated ingredient description here.", []);
        string newName = "Updated Ramen Bowl Soup";
        string newDescription = "This is an updated description for the delicious ramen soup bowl.";

        // Act
        product.Update(newName, newDescription, updatedCategory, [updatedIngredient], []);

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.Equal(newDescription, product.Description);
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
    public void Update_Should_ThrowDomainException_When_CategoryIsNull()
    {
        // Arrange
        Product product = CreateProduct();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() =>
            product.Update(DefaultName, DefaultDescription, null!, [DefaultIngredient], []));
        Assert.Equal(ProductErrors.CategoryRequired().Message, exception.Message);
    }

    private static Product CreateProduct(
        string? name = null,
        string? description = null,
        Collection<Ingredient>? baseIngredients = null,
        Collection<Category>? categories = null)
    {
        return Product.Create(
            DefaultId,
            DefaultCompanyId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            DefaultImageUrl,
            baseIngredients ?? [DefaultIngredient],
            [],
            categories ?? [DefaultCategory]);
    }

    private static void UpdateProduct(
        Product product,
        string? name = null,
        string? description = null,
        Category? category = null,
        IEnumerable<Ingredient>? ingredients = null,
        IEnumerable<Ingredient>? customIngredients = null)
    {
        product.Update(
            name ?? DefaultName,
            description ?? DefaultDescription,
            category ?? DefaultCategory,
            ingredients ?? [DefaultIngredient],
            customIngredients ?? []);
    }
}
