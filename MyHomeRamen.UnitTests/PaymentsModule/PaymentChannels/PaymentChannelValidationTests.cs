using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.PaymentChannel;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.UnitTests.PaymentsModule.PaymentChannels;

public sealed class PaymentChannelValidationTests
{
    private static readonly PaymentChannelId DefaultId = new(Guid.NewGuid());
    private static readonly PaymentGateway DefaultPaymentGateway = PaymentGateway.Create(new PaymentGatewayId(Guid.NewGuid()), "Stripe");
    private const string DefaultName = "Card";
    private const string DefaultImageUrl = "https://example.com/card.png";
    private const bool DefaultIsActive = true;
    private const int DefaultDisplayOrder = 1;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        PaymentChannel paymentChannel = PaymentChannel.Create(
            DefaultId,
            DefaultName,
            DefaultImageUrl,
            DefaultIsActive,
            DefaultDisplayOrder,
            DefaultPaymentGateway);

        // Assert
        Assert.Equal(DefaultId, paymentChannel.Id);
        Assert.Equal(DefaultName, paymentChannel.Name);
        Assert.Equal(DefaultImageUrl, paymentChannel.ImageUrl);
        Assert.Equal(DefaultIsActive, paymentChannel.IsActive);
        Assert.Equal(DefaultDisplayOrder, paymentChannel.DisplayOrder);
        Assert.Equal(DefaultPaymentGateway, paymentChannel.PaymentGateway);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentChannelConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentChannel(name: name));
        Assert.Equal(PaymentChannelErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentChannelConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentChannel(name: name));
        Assert.Equal(PaymentChannelErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_DisplayOrderIsTooSmall()
    {
        // Arrange
        int displayOrder = PaymentChannelConstants.MinSortOrder - 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentChannel(displayOrder: displayOrder));
        Assert.Equal(PaymentChannelErrors.SortOrderTooLow().Message, exception.Message);
    }

    private static PaymentChannel CreatePaymentChannel(
        string? name = null,
        string? imageUrl = null,
        bool? isActive = null,
        int? displayOrder = null,
        PaymentGateway? paymentGateway = null)
    {
        return PaymentChannel.Create(
            DefaultId,
            name ?? DefaultName,
            imageUrl ?? DefaultImageUrl,
            isActive ?? DefaultIsActive,
            displayOrder ?? DefaultDisplayOrder,
            paymentGateway ?? DefaultPaymentGateway);
    }
}
