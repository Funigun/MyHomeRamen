using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Domain.Orders.Products;

namespace MyHomeRamen.UnitTests.OrdersModule.Products;

public class ProductValidationTests
{
    private static readonly ProductId DefaultId = new(Guid.NewGuid());
    private static readonly ProductId DefaultOriginalId = new(Guid.NewGuid());
    private const string DefaultName = "Delicious Ramen";
    private const string DefaultDescription = "A very tasty ramen bowl that makes everyone happy and full.";
    private const decimal DefaultPrice = 25.0m;
    private const string DefaultImageUrl = "http://example.com/ramen.png";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        List<Ingredient> baseIngredients = [];
        List<Ingredient> customIngredients = [];

        // Act
        Product product = Product.Create(DefaultId, DefaultOriginalId, DefaultName, DefaultDescription, DefaultPrice, DefaultImageUrl, baseIngredients, customIngredients);

        // Assert
        Assert.Equal(DefaultId, product.Id);
        Assert.Equal(DefaultOriginalId, product.OriginalId);
        Assert.Equal(DefaultName, product.Name);
        Assert.Equal(DefaultDescription, product.Description);
        Assert.Equal(DefaultPrice, product.Price);
        Assert.Equal(DefaultImageUrl, product.ImageUrl);
        Assert.Equal(baseIngredients, product.BaseIngredients);
        Assert.Equal(customIngredients, product.CustomIngredients);
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
        List<Ingredient> baseIngredients = [ingredient, ingredient];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(baseIngredients: baseIngredients));
        Assert.Equal(ProductErrors.IngredientsNotUnique().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CustomIngredientsAreNotUnique()
    {
        // Arrange
        Ingredient ingredient = CreateIngredient();
        List<Ingredient> customIngredients = [ingredient, ingredient];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(customIngredients: customIngredients));
        Assert.Equal(ProductErrors.CustomIngredientsNotUnique().Message, exception.Message);
    }

    private static Product CreateProduct(
        string? name = null,
        string? description = null,
        decimal? price = null,
        List<Ingredient>? baseIngredients = null,
        List<Ingredient>? customIngredients = null)
    {
        return Product.Create(
            DefaultId,
            DefaultOriginalId,
            name ?? DefaultName,
            description ?? DefaultDescription,
            price ?? DefaultPrice,
            DefaultImageUrl,
            baseIngredients ?? [],
            customIngredients ?? []);
    }

    private static Ingredient CreateIngredient()
    {
        return Ingredient.Create(
            new IngredientId(Guid.NewGuid()),
            new IngredientId(Guid.NewGuid()),
            "Ingredient",
            "Description",
            10.0m);
    }
}
