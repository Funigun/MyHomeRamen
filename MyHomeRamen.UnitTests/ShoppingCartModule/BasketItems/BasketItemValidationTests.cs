using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.BasketItems;

public class BasketItemValidationTests
{
    private static BasketItem CreateBasketItem(
        Product? product = null,
        int quantity = 1,
        decimal price = 10m,
        string? comment = null)
    {
        BasketItemId id = new(Guid.NewGuid());
        product ??= Product.Create(new ProductId(Guid.NewGuid()), new ProductId(Guid.NewGuid()), "Test Product Name", "Test Desc", 10m, "url", [], []);

        return BasketItem.Create(id, product, quantity, price, comment);
    }

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        BasketItemId id = new(Guid.NewGuid());
        Product product = Product.Create(new ProductId(Guid.NewGuid()), new ProductId(Guid.NewGuid()), "Test Product", "Test Desc", 10m, "url", [], []);
        int quantity = 2;
        decimal price = 20m;
        string comment = "Some comment";

        // Act        BasketItem? item = BasketItem.Create(id, product, quantity, price, comment);

        // Assert
        Assert.Equal(id, item.Id);
        Assert.Equal(product, item.Product);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(price, item.Price);
        Assert.Equal(comment, item.Comment);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_ProductIsNull()
    {
        // Arrange & Act
        static BasketItem CreateBasketFunc() => CreateBasketItem(product: null!);

        // Assert
        DomainException? exception = Assert.Throws<DomainException>(CreateBasketFunc);
        Assert.Equal(BasketErrors.BasketItemProductRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_QuantityIsZero()
    {
        // Arrange & Act
        static BasketItem CreateBasketFunc() => CreateBasketItem(quantity: 0);

        // Assert
        DomainException? exception = Assert.Throws<DomainException>(CreateBasketFunc);
        Assert.Equal(BasketErrors.BasketItemQuantityInvalid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_QuantityIsNegative()
    {
        // Arrange & Act
        static BasketItem CreateBasketFunc() => CreateBasketItem(quantity: -1);

        // Assert
        DomainException? exception = Assert.Throws<DomainException>(CreateBasketFunc);
        Assert.Equal(BasketErrors.BasketItemQuantityInvalid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PriceIsNegative()
    {
        // Arrange & Act
        static BasketItem CreateBasketFunc() => CreateBasketItem(price: -1m);

        // Assert
        DomainException? exception = Assert.Throws<DomainException>(CreateBasketFunc);
        Assert.Equal(BasketErrors.BasketItemPriceInvalid().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_AllowNullComment_When_CommentIsNotProvided()
    {
        // Arrange & Act
        BasketItem? item = CreateBasketItem(comment: null);

        // Assert
        Assert.Null(item.Comment);
    }
}
