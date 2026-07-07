using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Users;

public sealed class UserRemoveAddressTests
{
    [Fact]
    public void RemoveAddress_Should_RemoveAddress_WhenExists()
    {
        // Arrange
        User user = CreateUser();
        Address address = CreateAddress(isDefault: false);
        user.AddAddress(address);

        // Act
        user.RemoveAddress(address.Id);

        // Assert
        Assert.Empty(user.Addresses);
    }

    [Fact]
    public void RemoveAddress_Should_ThrowDomainException_WhenAddressNotFound()
    {
        // Arrange
        User user = CreateUser();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => user.RemoveAddress(Guid.NewGuid()));
        Assert.Equal(AddressErrors.AddressNotFound().Message, exception.Message);
    }

    [Fact]
    public void RemoveAddress_Should_NotChangeOtherAddresses_WhenNonDefaultIsRemoved()
    {
        // Arrange
        User user = CreateUser();
        Address defaultAddress = CreateAddress(isDefault: true);
        Address nonDefaultAddress = CreateAddress(isDefault: false);
        user.AddAddress(defaultAddress);
        user.AddAddress(nonDefaultAddress);

        // Act
        user.RemoveAddress(nonDefaultAddress.Id);

        // Assert
        Assert.Single(user.Addresses);
        Assert.True(defaultAddress.IsDefault);
    }

    [Fact]
    public void RemoveAddress_Should_LeaveNoDefault_WhenDefaultAddressIsRemoved()
    {
        // Arrange
        User user = CreateUser();
        Address defaultAddress = CreateAddress(isDefault: true);
        Address otherAddress = CreateAddress(isDefault: false);
        user.AddAddress(defaultAddress);
        user.AddAddress(otherAddress);

        // Act
        user.RemoveAddress(defaultAddress.Id);

        // Assert
        Assert.Single(user.Addresses);
        Assert.False(otherAddress.IsDefault);
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
            role: "customer");
    }

    private static Address CreateAddress(bool isDefault = false)
    {
        return Address.Create(
            id: Guid.NewGuid(),
            street: "Main Street",
            building: "10A",
            apartment: "5",
            city: "Warsaw",
            zipCode: "00-001",
            isDefault: isDefault);
    }
}
