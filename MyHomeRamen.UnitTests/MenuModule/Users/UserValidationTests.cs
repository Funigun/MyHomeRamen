using System;
using System.Collections.Generic;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Menu.Users;
using Xunit;

namespace MyHomeRamen.UnitTests.MenuModule.Users;

public class UserValidationTests
{
    private static readonly UserId TestUserId = new(Guid.NewGuid());
    private static readonly Guid TestRestaurantId = Guid.NewGuid();

    private static readonly List<Permission> ValidPermissions =
    [
        Permission.Create(new PermissionId(Guid.NewGuid()), TestRestaurantId, PermissionConstants.CanViewProductsManagementView, "Permission description")
    ];

    private static readonly List<Role> ValidRoles =
    [
        Role.CreateEmployeeRole(new RoleId(Guid.NewGuid()), TestRestaurantId, ValidPermissions)
    ];

    [Fact]
    public void Create_Should_CreateUser_When_DataIsValid()
    {
        // Act
        User user = CreateUser();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(TestUserId, user.Id);
        Assert.Equal(TestRestaurantId, user.RestaurantId);
        Assert.Equal(ValidRoles, user.Roles);
        Assert.Equal(ValidPermissions, user.Permissions);
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

    [Fact]
    public void Create_Should_ThrowDomainException_When_PermissionNameIsInvalid()
    {
        // Arrange
        List<Permission> invalidPermissions =
        [
            Permission.Create(new PermissionId(Guid.NewGuid()), TestRestaurantId, "InvalidPermissionName", "Description")
        ];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(permissions: invalidPermissions));
        Assert.Equal(UserErrors.InvalidPermissionName().Message, exception.Message);
    }

    private static User CreateUser(
        List<Role>? roles = null,
        List<Permission>? permissions = null)
    {
        return User.Create(
            TestUserId,
            TestRestaurantId,
            roles ?? ValidRoles,
            permissions ?? ValidPermissions);
    }
}
