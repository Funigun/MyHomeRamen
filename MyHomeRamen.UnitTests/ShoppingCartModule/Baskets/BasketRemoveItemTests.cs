using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public sealed class BasketRemoveItemTests
{
    private static readonly BasketId DefaultBasketId = new(Guid.NewGuid());

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenItemExists()
    {
        // Arrange
        UserId userId = new(Guid.NewGuid());
        Basket basket = Basket.Create(DefaultBasketId, userId);
        BasketItem item = CreateBasketItem();
        basket.AddItem(item);

        // Act
        basket.RemoveItem(item.Id);

        // Assert
        Assert.Empty(basket.Items);
    }

    [Fact]
    public void RemoveItem_ShouldThrowDomainException_WhenItemNotFound()
    {
        // Arrange
        UserId userId = new(Guid.NewGuid());
        Basket basket = Basket.Create(DefaultBasketId, userId);
        BasketItemId nonExistentId = new(Guid.NewGuid());

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => basket.RemoveItem(nonExistentId));
        Assert.Equal(BasketErrors.ItemNotFound().Message, exception.Message);
    }

    private static BasketItem CreateBasketItem()
    {
        Product product = Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            "Test Product Name",
            "Test product description text that match minimum domain requirements",
            10m,
            "http://example.com/img.png",
            [],
            []);

        return BasketItem.Create(
            new BasketItemId(Guid.NewGuid()),
            product,
            1,
            null);
    }
}
