using System;
using System.Collections.Generic;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.PaymentGroup;
using MyHomeRamen.Domain.Payments.PaymentGroups;
using MyHomeRamen.Domain.Payments.PaymentProviders;
using Xunit;

namespace MyHomeRamen.UnitTests.PaymentsModule.PaymentGroups;

public class PaymentGroupValidationTests
{
    private static readonly PaymentGroupId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Credit Cards";
    private const string DefaultImageUrl = "http://example.com/credit-cards-group.png";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        PaymentProvider paymentProvider = CreatePaymentProvider();
        List<PaymentProvider> paymentProviders = [paymentProvider];

        // Act
        PaymentGroup paymentGroup = PaymentGroup.Create(DefaultId, DefaultName, DefaultImageUrl, paymentProviders);

        // Assert
        Assert.Equal(DefaultId, paymentGroup.Id);
        Assert.Equal(DefaultName, paymentGroup.Name);
        Assert.Equal(DefaultImageUrl, paymentGroup.ImageUrl);
        Assert.Equal(paymentProviders, paymentGroup.PaymentProviders);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentGroupConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentGroup(name: name));
        Assert.Equal(PaymentGroupErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentGroupConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentGroup(name: name));
        Assert.Equal(PaymentGroupErrors.NameTooLong().Message, exception.Message);
    }

    private static PaymentGroup CreatePaymentGroup(
        string? name = null,
        string? imageUrl = null,
        List<PaymentProvider>? paymentProviders = null)
    {
        return PaymentGroup.Create(
            DefaultId,
            name ?? DefaultName,
            imageUrl ?? DefaultImageUrl,
            paymentProviders ?? []);
    }

    private static PaymentProvider CreatePaymentProvider()
    {
        return PaymentProvider.Create(
            new PaymentProviderId(Guid.NewGuid()),
            "Stripe",
            "http://example.com/stripe.png");
    }
}
