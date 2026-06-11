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
        string? comment = null)
    {
        BasketItemId id = new(Guid.NewGuid());
        product ??= Product.Create(new ProductId(Guid.NewGuid()), new ProductId(Guid.NewGuid()), "Test Product Name", "Test Desc with valid description length matching domain requirements", 10m, "url", [], []);

        return BasketItem.Create(id, product, quantity, comment);
    }

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        BasketItemId id = new(Guid.NewGuid());
        Product product = Product.Create(new ProductId(Guid.NewGuid()), new ProductId(Guid.NewGuid()), "Test Product Name", "Test Desc with valid description length matching domain requirements", 10m, "url", [], []);
        int quantity = 2;
        string comment = "Some comment";

        // Act        BasketItem? item = BasketItem.Create(id, product, quantity, comment);

        // Assert
        Assert.Equal(id, item.Id);
        Assert.Equal(product, item.Product);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(product.TotalPrice * quantity, item.Price);
        Assert.Equal(comment, item.Comment);
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
    public void Create_Should_AllowNullComment_When_CommentIsNotProvided()
    {
        // Arrange & Act
        BasketItem? item = CreateBasketItem(comment: null);

        // Assert
        Assert.Null(item.Comment);
    }
}
