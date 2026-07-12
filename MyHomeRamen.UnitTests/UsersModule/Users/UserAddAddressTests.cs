using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Users;

public sealed class UserAddAddressTests
{
    [Fact]
    public void AddAddress_Should_AddAddress_When_UnderLimit()
    {
        // Arrange
        User user = CreateUser();
        Address address = CreateAddress(isDefault: true);

        // Act
        user.AddAddress(address);

        // Assert
        Assert.Single(user.Addresses);
    }

    [Fact]
    public void AddAddress_Should_ThrowDomainException_When_MaxAddressesReached()
    {
        // Arrange
        User user = CreateUser();
        for (int i = 0; i < AddressConstants.MaxAddressesPerUser; i++)
        {
            user.AddAddress(CreateAddress(isDefault: false));
        }

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => user.AddAddress(CreateAddress()));
        Assert.Equal(AddressErrors.MaxAddressesReached().Message, exception.Message);
    }

    [Fact]
    public void AddAddress_Should_SetNewAddressAsDefault_AndUnsetPreviousDefault()
    {
        // Arrange
        User user = CreateUser();
        Address firstAddress = CreateAddress(isDefault: true);
        user.AddAddress(firstAddress);

        Address newDefault = CreateAddress(isDefault: true);

        // Act
        user.AddAddress(newDefault);

        // Assert
        Assert.False(firstAddress.IsDefault);
        Assert.True(newDefault.IsDefault);
    }

    [Fact]
    public void AddAddress_Should_AllowNonDefaultAddress_WhenDefaultAlreadyExists()
    {
        // Arrange
        User user = CreateUser();
        Address defaultAddress = CreateAddress(isDefault: true);
        user.AddAddress(defaultAddress);

        Address nonDefault = CreateAddress(isDefault: false);

        // Act
        user.AddAddress(nonDefault);

        // Assert
        Assert.True(defaultAddress.IsDefault);
        Assert.False(nonDefault.IsDefault);
        Assert.Equal(2, user.Addresses.Count);
    }

    [Fact]
    public void AddAddress_Should_AutoSetDefault_WhenFirstAddress_AndIsDefaultFalse()
    {
        // Arrange
        User user = CreateUser();
        Address address = CreateAddress(isDefault: false);

        // Act
        user.AddAddress(address);

        // Assert
        Assert.True(address.IsDefault);
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
            role: Role.CreateForTest("customer"));
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
