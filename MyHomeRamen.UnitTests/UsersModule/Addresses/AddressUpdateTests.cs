using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Addresses;

public sealed class AddressUpdateTests
{
    private static readonly Guid DefaultId = Guid.NewGuid();
    private const string DefaultStreet = "Main Street";
    private const string DefaultBuilding = "10A";
    private const string DefaultApartment = "5";
    private const string DefaultCity = "Warsaw";
    private const string DefaultZipCode = "00-001";

    [Fact]
    public void Update_Should_UpdateFields_When_DataIsValid()
    {
        // Arrange
        Address address = CreateAddress();

        // Act
        address.Update("New Street", "20B", "10", "Krakow", "31-001");

        // Assert
        Assert.Equal("New Street", address.Street);
        Assert.Equal("20B", address.Building);
        Assert.Equal("10", address.Apartment);
        Assert.Equal("Krakow", address.City);
        Assert.Equal("31-001", address.ZipCode);
    }

    [Fact]
    public void Update_Should_NotChangeIsDefault_When_UpdateCalled()
    {
        // Arrange
        Address address = CreateAddress(isDefault: true);

        // Act
        address.Update("New Street", "20B", "10", "Krakow", "31-001");

        // Assert
        Assert.True(address.IsDefault);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_StreetIsEmpty()
    {
        // Arrange
        Address address = CreateAddress();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => address.Update(string.Empty, DefaultBuilding, DefaultApartment, DefaultCity, DefaultZipCode));
        Assert.Equal(AddressErrors.StreetRequired().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_StreetIsTooLong()
    {
        // Arrange
        Address address = CreateAddress();
        string street = new('a', AddressConstants.MaxStreetLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => address.Update(street, DefaultBuilding, DefaultApartment, DefaultCity, DefaultZipCode));
        Assert.Equal(AddressErrors.StreetTooLong().Message, exception.Message);
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_BuildingIsEmpty()
    {
        // Arrange
        Address address = CreateAddress();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => address.Update(DefaultStreet, string.Empty, DefaultApartment, DefaultCity, DefaultZipCode));
        Assert.Equal(AddressErrors.BuildingRequired().Message, exception.Message);
    }

    private static Address CreateAddress(bool isDefault = false)
    {
        return Address.Create(
            id: DefaultId,
            street: DefaultStreet,
            building: DefaultBuilding,
            apartment: DefaultApartment,
            city: DefaultCity,
            zipCode: DefaultZipCode,
            isDefault: isDefault);
    }
}
