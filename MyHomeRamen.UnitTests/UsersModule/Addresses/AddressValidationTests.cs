using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Addresses;

public sealed class AddressValidationTests
{
    private static readonly Guid DefaultId = Guid.NewGuid();
    private const string DefaultStreet = "Main Street";
    private const string DefaultBuilding = "10A";
    private const string DefaultApartment = "5";
    private const string DefaultCity = "Warsaw";
    private const string DefaultZipCode = "00-001";

    [Fact]
    public void Create_Should_CreateAddress_When_DataIsValid()
    {
        // Act
        Address address = CreateAddress();

        // Assert
        Assert.Equal(DefaultId, address.Id);
        Assert.Equal(DefaultStreet, address.Street);
        Assert.Equal(DefaultBuilding, address.Building);
        Assert.Equal(DefaultApartment, address.Apartment);
        Assert.Equal(DefaultCity, address.City);
        Assert.Equal(DefaultZipCode, address.ZipCode);
    }

    [Fact]
    public void Create_Should_SetIsDefault_When_IsDefaultTrue()
    {
        // Act
        Address address = CreateAddress(isDefault: true);

        // Assert
        Assert.True(address.IsDefault);
    }

    [Fact]
    public void Create_Should_NotSetIsDefault_When_IsDefaultFalse()
    {
        // Act
        Address address = CreateAddress(isDefault: false);

        // Assert
        Assert.False(address.IsDefault);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_StreetIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(street: string.Empty));
        Assert.Equal(AddressErrors.StreetRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_StreetIsTooLong()
    {
        // Arrange
        string street = new('a', AddressConstants.MaxStreetLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(street: street));
        Assert.Equal(AddressErrors.StreetTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_BuildingIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(building: string.Empty));
        Assert.Equal(AddressErrors.BuildingRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_BuildingIsTooLong()
    {
        // Arrange
        string building = new('a', AddressConstants.MaxBuildingLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(building: building));
        Assert.Equal(AddressErrors.BuildingTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_ApartmentIsTooLong()
    {
        // Arrange
        string apartment = new('a', AddressConstants.MaxApartmentLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(apartment: apartment));
        Assert.Equal(AddressErrors.ApartmentTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CityIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(city: string.Empty));
        Assert.Equal(AddressErrors.CityRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_CityIsTooLong()
    {
        // Arrange
        string city = new('a', AddressConstants.MaxCityLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(city: city));
        Assert.Equal(AddressErrors.CityTooLong().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_ZipCodeIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(zipCode: string.Empty));
        Assert.Equal(AddressErrors.ZipCodeRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_ZipCodeIsTooLong()
    {
        // Arrange
        string zipCode = new('a', AddressConstants.MaxZipCodeLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateAddress(zipCode: zipCode));
        Assert.Equal(AddressErrors.ZipCodeTooLong().Message, exception.Message);
    }

    private static Address CreateAddress(
        string? street = null,
        string? building = null,
        string? apartment = null,
        string? city = null,
        string? zipCode = null,
        bool isDefault = false)
    {
        return Address.Create(
            DefaultId,
            street ?? DefaultStreet,
            building ?? DefaultBuilding,
            apartment ?? DefaultApartment,
            city ?? DefaultCity,
            zipCode ?? DefaultZipCode,
            isDefault);
    }
}
