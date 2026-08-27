using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

internal sealed record TestUserData(User User, Role Role)
{
    internal (string KeycloakUserId, Guid UserId) Authorization => (User.KeycloakUserId!, User.Id);
}

internal static class TestUserFactory
{
    internal static async Task<TestUserData> CreateAsync(
        IIdentityDbContext dbContext,
        string roleName,
        IEnumerable<string> permissionNames,
        string userName,
        string firstName = "Test",
        string lastName = "User")
    {
        IEnumerable<Permission> permissions = await dbContext.Permission.Load().All(TestContext.Current.CancellationToken);
        IEnumerable<PermissionId> permissionIds = permissions
            .Where(permission => permissionNames.Contains(permission.Name))
            .Select(permission => permission.Id);

        Role role = Role.Create(roleName, $"{roleName} role for testing purposes", permissionIds);
        string keycloakUserId = $"test-keycloak-{Guid.NewGuid():N}";
        User user = User.Create(
            keycloakUserId,
            userName,
            firstName,
            lastName,
            $"{userName}@example.com",
            "123456789",
            role);

        dbContext.Role.Add(role);
        dbContext.User.Add(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return new TestUserData(user, role);
    }

    internal static async Task DeleteAsync(IIdentityDbContext dbContext, IEnumerable<TestUserData> users)
    {
        foreach (TestUserData testUser in users)
        {
            dbContext.User.Delete(testUser.User);
            dbContext.Role.Delete(testUser.Role);
        }

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
