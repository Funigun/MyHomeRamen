using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketValidationTests
{
    private static readonly BasketId DefaultId = new(Guid.NewGuid());

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        User user = CreateUser();

        // Act
        Basket basket = Basket.Create(DefaultId, user);

        // Assert
        Assert.Equal(DefaultId, basket.Id);
        Assert.Equal(user, basket.User);
        Assert.Empty(basket.Products);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_UserIsNull()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Basket.Create(DefaultId, null!));
        Assert.Equal(BasketErrors.BasketUserRequired().Message, exception.Message);
    }

    private static User CreateUser()
    {
        return User.Create(
            new UserId(Guid.NewGuid()),
            [],
            []);
    }
}
