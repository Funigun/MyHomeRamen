using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.PaymentMethod;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.UnitTests.PaymentsModule.PaymentMethods;

public sealed class PaymentMethodValidationTests
{
    private static readonly PaymentMethodId DefaultId = new(Guid.NewGuid());
    private const string DefaultName = "Credit Card";
    private const string DefaultImageUrl = "https://example.com/credit-card.png";
    private const bool DefaultIsActive = true;
    private const int DefaultDisplayOrder = 1;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        PaymentMethod paymentMethod = PaymentMethod.Create(
            DefaultId,
            DefaultName,
            DefaultImageUrl,
            DefaultIsActive,
            DefaultDisplayOrder);

        // Assert
        Assert.Equal(DefaultId, paymentMethod.Id);
        Assert.Equal(DefaultName, paymentMethod.Name);
        Assert.Equal(DefaultImageUrl, paymentMethod.ImageUrl);
        Assert.Equal(DefaultIsActive, paymentMethod.IsActive);
        Assert.Equal(DefaultDisplayOrder, paymentMethod.DisplayOrder);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooShort()
    {
        // Arrange
        string name = new('a', PaymentMethodConstants.MinNameLength - 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentMethod(name: name));
        Assert.Equal(PaymentMethodErrors.NameTooShort().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NameIsTooLong()
    {
        // Arrange
        string name = new('a', PaymentMethodConstants.MaxNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentMethod(name: name));
        Assert.Equal(PaymentMethodErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_DisplayOrderIsTooSmall()
    {
        // Arrange
        int displayOrder = PaymentMethodConstants.MinSortOrder - 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreatePaymentMethod(displayOrder: displayOrder));
        Assert.Equal(PaymentMethodErrors.SortOrderTooLow().Message, exception.Message);
    }

    private static PaymentMethod CreatePaymentMethod(
        string? name = null,
        string? imageUrl = null,
        bool? isActive = null,
        int? displayOrder = null)
    {
        return PaymentMethod.Create(
            DefaultId,
            name ?? DefaultName,
            imageUrl ?? DefaultImageUrl,
            isActive ?? DefaultIsActive,
            displayOrder ?? DefaultDisplayOrder);
    }
}
