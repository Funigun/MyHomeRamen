using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public sealed class BasketClearTests
{
    private static readonly BasketId DefaultBasketId = new(Guid.NewGuid());

    [Fact]
    public void Clear_ShouldRemoveAllItems_WhenBasketHasItems()
    {
        // Arrange
        UserId userId = new(Guid.NewGuid());
        Basket basket = Basket.Create(DefaultBasketId, userId);
        basket.AddItem(CreateBasketItem());
        basket.AddItem(CreateBasketItem());

        // Act
        basket.Clear();

        // Assert
        Assert.Empty(basket.Items);
    }

    [Fact]
    public void Clear_ShouldDoNothing_WhenBasketIsEmpty()
    {
        // Arrange
        UserId userId = new(Guid.NewGuid());
        Basket basket = Basket.Create(DefaultBasketId, userId);

        // Act
        basket.Clear();

        // Assert
        Assert.Empty(basket.Items);
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
