using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Order;
using MyHomeRamen.Domain.Payments.Orders;

namespace MyHomeRamen.UnitTests.PaymentsModule.Orders;

public class OrderValidationTests
{
    private static readonly OrderId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();
    private static readonly OrderId DefaultOriginalId = new(Guid.NewGuid());
    private const decimal DefaultAmount = 50.0m;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Order order = Order.Create(DefaultId, DefaultRestaurantId, DefaultOriginalId, DefaultAmount);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(DefaultOriginalId, order.OriginalId);
        Assert.Equal(DefaultAmount, order.Amount);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_AmountIsTooSmall()
    {
        // Arrange
        decimal amount = OrderConstants.MinTotalAmount - 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateOrder(amount: amount));
        Assert.Equal(OrderErrors.AmountTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_AmountIsTooLarge()
    {
        // Arrange
        decimal amount = OrderConstants.MaxTotalAmount + 0.01m;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateOrder(amount: amount));
        Assert.Equal(OrderErrors.AmountTooLarge().Message, exception.Message);
    }

    private static Order CreateOrder(decimal? amount = null)
    {
        return Order.Create(
            DefaultId,
            DefaultRestaurantId,
            DefaultOriginalId,
            amount ?? DefaultAmount);
    }
}
