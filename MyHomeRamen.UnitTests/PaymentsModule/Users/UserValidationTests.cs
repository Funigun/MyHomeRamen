using System;
using System.Collections.Generic;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Payments.Payments;
using MyHomeRamen.Domain.Payments.Users;
using Xunit;

namespace MyHomeRamen.UnitTests.PaymentsModule.Users;

public class UserValidationTests
{
    private static readonly UserId TestUserId = new(Guid.NewGuid());
    private static readonly Guid TestRestaurantId = Guid.NewGuid();
    private static readonly PaymentId TestPaymentId = new(Guid.NewGuid());
    private static readonly Guid TestReferenceId = Guid.NewGuid();
    
    private static readonly Payment TestPayment = Payment.Create(
        TestPaymentId,
        TestRestaurantId,
        TestReferenceId,
        "Credit Card",
        "https://example.com/image.png");

    private static readonly List<Permission> ValidPermissions =
    [
        Permission.Create(new PermissionId(Guid.NewGuid()), TestRestaurantId, PermissionConstants.CanViewPayments, "Permission description")
    ];

    private static readonly List<Role> ValidRoles =
    [
        Role.CreateCustomerRole(new RoleId(Guid.NewGuid()), TestRestaurantId, ValidPermissions)
    ];

    [Fact]
    public void Create_Should_CreateUser_When_DataIsValid()
    {
        // Act
        User user = CreateUser();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(TestUserId, user.Id);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("1234567890", user.PhoneNumber);
        Assert.Equal(TestPayment, user.DefaultMethod);
        Assert.Equal(ValidRoles, user.Roles);
        Assert.Equal(ValidPermissions, user.Permissions);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_FirstNameIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(firstName: ""));
        Assert.Equal(UserErrors.FirstNameRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_LastNameIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(lastName: ""));
        Assert.Equal(UserErrors.LastNameRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_EmailIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(email: ""));
        Assert.Equal(UserErrors.EmailRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PhoneNumberIsEmpty()
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(phoneNumber: ""));
        Assert.Equal(UserErrors.PhoneNumberRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_RolesListIsEmpty()
    {
        // Arrange
        List<Role> emptyRoles = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(roles: emptyRoles));
        Assert.Equal(UserErrors.MissingRole().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PermissionsListIsEmpty()
    {
        // Arrange
        List<Permission> emptyPermissions = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(permissions: emptyPermissions));
        Assert.Equal(UserErrors.MissingPermission().Message, exception.Message);
    }

    private static User CreateUser(
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@example.com",
        string phoneNumber = "1234567890",
        List<Role>? roles = null,
        List<Permission>? permissions = null)
    {
        return User.Create(
            TestUserId,
            TestRestaurantId,
            firstName,
            lastName,
            email,
            phoneNumber,
            TestPayment,
            roles ?? ValidRoles,
            permissions ?? ValidPermissions);
    }
}
