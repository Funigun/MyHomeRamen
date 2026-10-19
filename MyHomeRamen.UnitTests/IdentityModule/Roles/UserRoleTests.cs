using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.IdentityModule.Roles;

public sealed class UserRoleTests
{
    [Fact]
    public void AssignRole_ShouldCreateAssociation_WhenRoleIsNotAssigned()
    {
        User user = CreateUser();
        Role role = Role.CreateCustom("Customer", "Customer role");

        user.AssignRole(role);

        UserRole userRole = Assert.Single(user.UserRoles);
        Assert.Equal(user.Id, userRole.UserId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public void AssignRole_ShouldRejectDuplicateRole_WhenAlreadyAssigned()
    {
        User user = CreateUser();
        Role role = Role.CreateCustom("Customer", "Customer role");
        user.AssignRole(role);

        DomainException exception = Assert.Throws<DomainException>(() => user.AssignRole(role));

        Assert.Equal(UserRoleErrors.RoleAlreadyAssigned().Message, exception.Message);
    }

    private static User CreateUser()
    {
        return User.Create("keycloak-user-id", "test-user", "Test", "User", "test@example.com", "123456789");
    }
}
