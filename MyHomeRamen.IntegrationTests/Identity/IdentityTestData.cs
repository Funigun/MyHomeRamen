using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;

namespace MyHomeRamen.IntegrationTests.Identity;

public class IdentityTestData
{
    public (string KeycloakUserId, Guid UserId) AdminUser { get; private set;  }
    public (string KeycloakUserId, Guid UserId) GuestUser { get; private set; }
    public (string KeycloakUserId, Guid UserId) EmployeeUser { get; private set; }
    public (string KeycloakUserId, Guid UserId) ManagerUser { get; private set; }
    public (string KeycloakUserId, Guid UserId) CustomerUser { get; private set; }

    private IIdentityDbContext identityDbContext = null!;

    public async Task SeedAsync(IServiceScope seedScope)
    {
        identityDbContext = seedScope.ServiceProvider.GetRequiredService<IIdentityDbContext>();

        IPermissionCatalogSynchronizer permissionCatalogSynchronizer = seedScope.ServiceProvider.GetRequiredService<IPermissionCatalogSynchronizer>();
        await permissionCatalogSynchronizer.Synchronize(TestContext.Current.CancellationToken);

        IReadOnlyCollection<Permission> permissions = await identityDbContext.Permission.Query().All(TestContext.Current.CancellationToken);
        IReadOnlyCollection<PermissionId> allPermissionIds = permissions.Select(permission => permission.Id).ToArray();

        Role adminRole = await identityDbContext.Role.Load().ByName(RoleConstants.Admin, TestContext.Current.CancellationToken);
        Role guestRole = await identityDbContext.Role.Load().ByName(RoleConstants.Guest, TestContext.Current.CancellationToken);
        Role employeeRole = Role.Create(RoleConstants.Employee, "Employee role for testing purposes", GetProfilePermissionIds(permissions));
        Role managerRole = Role.Create(RoleConstants.Manager, "Manager role for testing purposes", allPermissionIds);
        Role customerRole = Role.Create(RoleConstants.Customer, "Customer role for testing purposes", GetProfilePermissionIds(permissions));

        User adminUser = CreateUser("admin", "Admin", adminRole);
        User employeeUser = CreateUser("employee", "Employee", employeeRole);
        User managerUser = CreateUser("manager", "Manager", managerRole);
        User customerUser = CreateUser("customer", "Customer", customerRole);
        User guestUser = User.CreateGuest();

        identityDbContext.Role.AddRange([employeeRole, managerRole, customerRole]);
        identityDbContext.User.AddRange([adminUser, guestUser, employeeUser, managerUser, customerUser]);
        await identityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        AdminUser = (adminUser.KeycloakUserId!, adminUser.Id);
        GuestUser = (guestUser.KeycloakUserId!, guestUser.Id);
        EmployeeUser = (employeeUser.KeycloakUserId!, employeeUser.Id);
        ManagerUser = (managerUser.KeycloakUserId!, managerUser.Id);
        CustomerUser = (customerUser.KeycloakUserId!, customerUser.Id);
    }

    private static User CreateUser(string userName, string name, Role role)
    {
        return User.Create(
            keycloakUserId: $"test-keycloak-{userName}",
            userName,
            firstName: name,
            lastName: "User",
            email: $"{userName}@example.com",
            phoneNumber: "123456789",
            role);
    }

    private IEnumerable<PermissionId> GetProfilePermissionIds(IEnumerable<Permission> permissions)
    {
        string[] profilePermissions =
        [
            PermissionConstants.CanViewUserProfile,
            PermissionConstants.CanUpdateUserProfile,
            PermissionConstants.CanDeleteUserProfile
        ];

        return permissions.Where(permission => profilePermissions.Contains(permission.Name))
                          .Select(permission => permission.Id)
                          .ToArray();
    }

    public async Task<(string KeycloakId, Guid UserId)> SeedUser(IEnumerable<string> permissions, string module)
    {
        IEnumerable<Permission> permissionEntities = permissions.Select(p => Permission.Create(p, "Test permission", module));
        Role role = Role.Create($"TestRole-{Guid.NewGuid()}", "Test role for testing purposes", permissionEntities.Select(p => p.Id));

        string keycloakUserId = $"test-keycloak-{Guid.NewGuid()}";

        User user = User.Create(keycloakUserId, "test", "test", "user", "test@example.com", "123456789", role);

        identityDbContext.Permission.AddRange(permissionEntities);
        identityDbContext.Role.Add(role);
        identityDbContext.User.Add(user);
        await identityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (keycloakUserId, user.Id);
    }
}
