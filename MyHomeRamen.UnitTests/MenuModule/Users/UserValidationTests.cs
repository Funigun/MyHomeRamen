using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.UnitTests.MenuModule.Users;

public class UserValidationTests
{
    private static readonly UserId TestUserId = new(Guid.NewGuid());

    private static readonly List<Permission> ValidPermissions =
    [
        Permission.Create(new PermissionId(Guid.NewGuid()), PermissionConstants.CanViewProductsManagementView, "Permission description")
    ];

    private static readonly List<Role> ValidRoles =
    [
        Role.CreateEmployeeRole(new RoleId(Guid.NewGuid()), ValidPermissions)
    ];

    [Fact]
    public void Create_Should_CreateUser_When_DataIsValid()
    {
        // Act
        User user = CreateUser();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(TestUserId, user.Id);
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
            Permission.Create(new PermissionId(Guid.NewGuid()), "InvalidPermissionName", "Description")
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
            roles ?? ValidRoles,
            permissions ?? ValidPermissions);
    }
}
