using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketValidationTests
{
    private static readonly BasketId DefaultId = new(Guid.NewGuid());

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        UserId userId = new(Guid.NewGuid());
        Guid restaurantId = Guid.NewGuid();

        // Act
        Basket basket = Basket.Create(DefaultId, userId, restaurantId);

        // Assert
        Assert.Equal(DefaultId, basket.Id);
        Assert.Equal(userId, basket.UserId);
        Assert.Equal(restaurantId, basket.RestaurantId);
        Assert.Empty(basket.Items);
        Assert.Equal(BasketStatus.Active, basket.Status);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_UserIsNull()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Basket.Create(DefaultId, Guid.Empty, Guid.NewGuid()));
        Assert.Equal(BasketErrors.BasketUserRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_RestaurantIsEmpty()
    {
        DomainException exception = Assert.Throws<DomainException>(() => Basket.Create(DefaultId, Guid.NewGuid(), Guid.Empty));
        Assert.Equal(BasketErrors.BasketRestaurantRequired().Message, exception.Message);
    }
}
