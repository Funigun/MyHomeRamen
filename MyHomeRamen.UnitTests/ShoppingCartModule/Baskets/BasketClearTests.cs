using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using Xunit;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public sealed class BasketClearTests
{
    private static readonly BasketId DefaultBasketId = new(Guid.NewGuid());

    [Fact]
    public void Clear_ShouldRemoveAllItems_WhenBasketHasItems()
    {
        // Arrange
        User user = CreateUser();
        Basket basket = Basket.Create(DefaultBasketId, user);
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
        User user = CreateUser();
        Basket basket = Basket.Create(DefaultBasketId, user);

        // Act
        basket.Clear();

        // Assert
        Assert.Empty(basket.Items);
    }

    private static User CreateUser()
        => User.Create(new UserId(Guid.NewGuid()), [], []);

    private static BasketItem CreateBasketItem()
    {
        Product product = Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            "Test Product Name",
            "Test product description text",
            10m,
            "http://example.com/img.png",
            [],
            []);

        return BasketItem.Create(
            new BasketItemId(Guid.NewGuid()),
            product,
            1,
            10m,
            null);
    }
}
