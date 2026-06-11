using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.PaymentGateway;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.UnitTests.PaymentsModule.PaymentGateways;

public sealed class PaymentGatewayValidationTests
{
    private static readonly PaymentGatewayId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Stripe";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        PaymentGateway paymentGateway = PaymentGateway.Create(DefaultId, DefaultName);

        // Assert
        Assert.Equal(DefaultId, paymentGateway.Id);
        Assert.Equal(DefaultName, paymentGateway.Name);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentGatewayConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentGateway(name: name));
        Assert.Equal(PaymentGatewayErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentGatewayConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentGateway(name: name));
        Assert.Equal(PaymentGatewayErrors.NameTooLong().Message, exception.Message);
    }

    private static PaymentGateway CreatePaymentGateway(string? name = null)
    {
        return PaymentGateway.Create(DefaultId, name ?? DefaultName);
    }
}
