using System;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Payment;
using MyHomeRamen.Domain.Payments.Payments;
using Xunit;

namespace MyHomeRamen.UnitTests.PaymentsModule.Payments;

public class PaymentValidationTests
{
    private static readonly PaymentId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Credit Card";
    private const string DefaultImageUrl = "http://example.com/credit-card.png";

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Payment payment = Payment.Create(DefaultId, DefaultName, DefaultImageUrl);

        // Assert
        Assert.Equal(DefaultId, payment.Id);
        Assert.Equal(DefaultName, payment.Name);
        Assert.Equal(DefaultImageUrl, payment.ImageUrl);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePayment(name: name));
        Assert.Equal(PaymentErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePayment(name: name));
        Assert.Equal(PaymentErrors.NameTooLong().Message, exception.Message);
    }

    private static Payment CreatePayment(
        string? name = null,
        string? imageUrl = null)
    {
        return Payment.Create(
            DefaultId,
            name ?? DefaultName,
            imageUrl ?? DefaultImageUrl);
    }
}
