using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketBehaviorTests
{
    private static readonly BasketId DefaultBasketId = new(Guid.NewGuid());

    [Fact]
    public void AddItem_ShouldAddItemToBasket_WhenValidItem()
    {
        // Arrange
        User user = CreateUser();
        Basket basket = Basket.Create(DefaultBasketId, user);
        BasketItem item = CreateBasketItem();

        // Act
        basket.AddItem(item);

        // Assert
        Assert.Single(basket.Items);
        Assert.Contains(item, basket.Items);
    }

    [Fact]
    public void AddItem_ShouldThrowDomainException_WhenItemIsNull()
    {
        // Arrange
        User user = CreateUser();
        Basket basket = Basket.Create(DefaultBasketId, user);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => basket.AddItem(null!));
        Assert.Equal(BasketErrors.BasketItemProductRequired().Message, exception.Message);
    }

    [Fact]
    public void AddItem_ShouldThrowDomainException_WhenItemsLimitReached()
    {
        // Arrange
        User user = CreateUser();
        Basket basket = Basket.Create(DefaultBasketId, user);

        for (int i = 0; i < BasketConstants.MaxProductsCount; i++)
        {
            basket.AddItem(CreateBasketItem());
        }

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => basket.AddItem(CreateBasketItem()));
        Assert.Equal(BasketErrors.BasketItemsLimitReached().Message, exception.Message);
    }

    private static User CreateUser()
        => User.Create(new UserId(Guid.NewGuid()), [], []);

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
