using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public class BasketTests
{
    [Fact]
    public void BasketMaxValue_MustBeEqualTo_OrderMaxPrice()
    {
        // Arrange
        decimal basketMaxValue = BasketConstants.MaxTotalPrice;
        decimal orderMaxValue = OrderConstants.MaxTotalAmount;

        // Act & Assert
        Assert.Equal(basketMaxValue, orderMaxValue);
    }
}
