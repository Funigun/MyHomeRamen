using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;

namespace MyHomeRamen.IntegrationTests.Identity;

public static class IdentityTestData
{
    public static Guid AdminUserId { get; private set; }

    public static Guid GuestUserId { get; private set; }

    public static Guid GuestId { get; private set; }

    public static Guid EmployeeUserId { get; private set; }

    public static Guid ManagerUserId { get; private set; }

    public static Guid CustomerUserId { get; private set; }

    public static async Task SeedAsync(IServiceScope seedScope)
    {
        IIdentityDbContext identityDbContext = seedScope.ServiceProvider.GetRequiredService<IIdentityDbContext>();

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

        identityDbContext.Role.AddRange([adminRole, guestRole, employeeRole, managerRole, customerRole]);
        identityDbContext.User.AddRange([adminUser, guestUser, employeeUser, managerUser, customerUser]);
        await identityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        AdminUserId = adminUser.Id;
        GuestUserId = guestUser.Id;
        GuestId = guestUser.GuestId!.Value;
        EmployeeUserId = employeeUser.Id;
        ManagerUserId = managerUser.Id;
        CustomerUserId = customerUser.Id;
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

    private static IEnumerable<PermissionId> GetProfilePermissionIds(IEnumerable<Permission> permissions)
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
}
