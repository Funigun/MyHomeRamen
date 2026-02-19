using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.PaymentProvider;
using MyHomeRamen.Domain.Payments.PaymentProviders;

namespace MyHomeRamen.UnitTests.PaymentsModule.PaymentProviders;

public class PaymentProviderValidationTests
{
    private static readonly PaymentProviderId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();
    private const string DefaultName = "Stripe";
    private const string DefaultImageUrl = "http://example.com/stripe.png";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        PaymentProvider paymentProvider = PaymentProvider.Create(DefaultId, DefaultRestaurantId, DefaultName, DefaultImageUrl);

        // Assert
        Assert.Equal(DefaultId, paymentProvider.Id);
        Assert.Equal(DefaultName, paymentProvider.Name);
        Assert.Equal(DefaultImageUrl, paymentProvider.ImageUrl);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentProviderConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentProvider(name: name));
        Assert.Equal(PaymentProviderErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentProviderConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentProvider(name: name));
        Assert.Equal(PaymentProviderErrors.NameTooLong().Message, exception.Message);
    }

    private static PaymentProvider CreatePaymentProvider(
        string? name = null,
        string? imageUrl = null)
    {
        return PaymentProvider.Create(
            DefaultId,
            DefaultRestaurantId,
            name ?? DefaultName,
            imageUrl ?? DefaultImageUrl);
    }
}
