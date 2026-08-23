using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Users;

public sealed class UserUpdateAddressTests
{
    [Fact]
    public void UpdateAddress_Should_UpdateAddress_When_Valid()
    {
        // Arrange
        User user = CreateUser();
        Address address = CreateAddress();
        user.AddAddress(address);

        // Act
        user.UpdateAddress(address.Id, "New Street", "20B", "10", "Krakow", "31-001", isDefault: false);

        // Assert
        Address updated = user.Addresses.First(a => a.Id == address.Id);
        Assert.Equal("New Street", updated.Street);
        Assert.Equal("20B", updated.Building);
        Assert.Equal("10", updated.Apartment);
        Assert.Equal("Krakow", updated.City);
        Assert.Equal("31-001", updated.ZipCode);
    }

    [Fact]
    public void UpdateAddress_Should_SetAsDefault_AndUnsetPreviousDefault()
    {
        // Arrange
        User user = CreateUser();
        Address firstAddress = CreateAddress(isDefault: true);
        Address secondAddress = CreateAddress(isDefault: false);
        user.AddAddress(firstAddress);
        user.AddAddress(secondAddress);

        // Act
        user.UpdateAddress(secondAddress.Id, secondAddress.Street, secondAddress.Building, secondAddress.Apartment, secondAddress.City, secondAddress.ZipCode, isDefault: true);

        // Assert
        Assert.False(firstAddress.IsDefault);
        Assert.True(secondAddress.IsDefault);
    }

    [Fact]
    public void UpdateAddress_Should_UnsetDefault_WhenIsDefaultFalse_OnCurrentDefault()
    {
        // Arrange
        User user = CreateUser();
        Address address = CreateAddress(isDefault: true);
        user.AddAddress(address);

        // Act
        user.UpdateAddress(address.Id, address.Street, address.Building, address.Apartment, address.City, address.ZipCode, isDefault: false);

        // Assert
        Assert.False(address.IsDefault);
    }

    [Fact]
    public void UpdateAddress_Should_NotChangeDefault_WhenIsDefaultFalse_OnNonDefault()
    {
        // Arrange
        User user = CreateUser();
        Address defaultAddress = CreateAddress(isDefault: true);
        Address nonDefaultAddress = CreateAddress(isDefault: false);
        user.AddAddress(defaultAddress);
        user.AddAddress(nonDefaultAddress);

        // Act
        user.UpdateAddress(nonDefaultAddress.Id, nonDefaultAddress.Street, nonDefaultAddress.Building, nonDefaultAddress.Apartment, nonDefaultAddress.City, nonDefaultAddress.ZipCode, isDefault: false);

        // Assert
        Assert.True(defaultAddress.IsDefault);
        Assert.False(nonDefaultAddress.IsDefault);
    }

    [Fact]
    public void UpdateAddress_Should_ThrowDomainException_When_AddressNotFound()
    {
        // Arrange
        User user = CreateUser();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() =>
            user.UpdateAddress(Guid.NewGuid(), "Street", "1A", string.Empty, "Warsaw", "00-001", isDefault: false));
        Assert.Equal(AddressErrors.AddressNotFound().Message, exception.Message);
    }

    private static User CreateUser()
    {
        return User.Create(
            keycloakUserId: Guid.NewGuid().ToString(),
            userName: "testuser",
            firstName: "Test",
            lastName: "User",
            email: "test@example.com",
            phoneNumber: "123456789",
            role: Role.Create("customer", "customer role"));
    }

    private static Address CreateAddress(bool isDefault = false)
    {
        return Address.Create(
            street: "Main Street",
            building: "10A",
            apartment: "5",
            city: "Warsaw",
            zipCode: "00-001",
            isDefault: isDefault);
    }
}
