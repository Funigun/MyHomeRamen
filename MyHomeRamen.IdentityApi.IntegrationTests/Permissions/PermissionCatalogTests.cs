using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Permissions;

public sealed class PermissionCatalogTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task Synchronize_ShouldGrantOwnerCompanyPermissionsAndAllPermissionsToAdmin()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        IReadOnlyCollection<Permission> permissions =
            (await apiFactory.IdentityDbContext.Permission.Load().All(cancellationToken)).ToArray();
        Role? companyOwnerRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.CompanyOwner, cancellationToken);
        Role? adminRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.RestaurantAdmin, cancellationToken);

        Assert.NotNull(companyOwnerRole);
        Assert.NotNull(adminRole);
        Assert.Null(companyOwnerRole.RestaurantId);
        Assert.Null(adminRole.RestaurantId);

        string[] actualCompanyOwnerPermissions = companyOwnerRole.RolePermissions
            .Select(rolePermission => permissions.Single(permission => permission.Id == rolePermission.PermissionId))
            .Select(permission => $"{permission.Module}:{permission.Name}")
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();
        string[] expectedCompanyOwnerPermissions = RestaurantsPermissionConstants.CompanyOwnerPermissions
            .Select(permission => $"Restaurants:{permission}")
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();
        string[] actualAdminPermissions = adminRole.RolePermissions
            .Select(rolePermission => permissions.Single(permission => permission.Id == rolePermission.PermissionId))
            .Select(permission => $"{permission.Module}:{permission.Name}")
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();
        string[] expectedAdminPermissions = permissions
            .Select(permission => $"{permission.Module}:{permission.Name}")
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedCompanyOwnerPermissions, actualCompanyOwnerPermissions);
        Assert.Equal(expectedAdminPermissions, actualAdminPermissions);

        HashSet<string> resourceScopedPermissionNames =
            RestaurantsPermissionConstants.ResourceScopedPermissions.ToHashSet(StringComparer.Ordinal);
        foreach (Permission restaurantPermission in permissions.Where(permission => permission.Module == "Restaurants"))
        {
            Assert.Equal(
                resourceScopedPermissionNames.Contains(restaurantPermission.Name),
                restaurantPermission.IsResourceScoped);
        }
    }

    [Fact]
    public async Task ByUserId_ShouldPreserveRestaurantScope_WhenUserHasScopedRole()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Permission? permission = await apiFactory.IdentityDbContext.Permission.Load()
            .ByModuleAndName("Restaurants", RestaurantsPermissionConstants.RestaurantManage, cancellationToken);
        Assert.NotNull(permission);

        Guid restaurantId = Guid.CreateVersion7();
        string identityId = $"test-keycloak-{Guid.CreateVersion7()}";
        string userName = $"scoped-{Guid.CreateVersion7()}";
        Role role = Role.CreateCustom(
            $"Scoped Permission {Guid.CreateVersion7()}",
            "Test role for restaurant-scoped permission.",
            [permission.Id],
            restaurantId);
        User user = User.Create(identityId, userName, "Scoped", "User", $"{userName}@example.com", "123456789");
        user.AssignRole(role);

        apiFactory.IdentityDbContext.Role.Add(role);
        apiFactory.IdentityDbContext.User.Add(user);
        await apiFactory.IdentityDbContext.SaveChangesAsync(cancellationToken);

        try
        {
            IReadOnlyCollection<PermissionGrant> grants = await apiFactory.IdentityDbContext.Permission.Query()
                .ByUserId(user.Id.Value, cancellationToken);
            PermissionGrant grant = Assert.Single(grants, candidate =>
                candidate.Module == "Restaurants"
                && candidate.Name == RestaurantsPermissionConstants.RestaurantManage);

            Assert.True(grant.IsResourceScoped);
            Assert.Equal(restaurantId, grant.RestaurantId);
        }
        finally
        {
            apiFactory.IdentityDbContext.User.Delete(user);
            apiFactory.IdentityDbContext.Role.Delete(role);
            await apiFactory.IdentityDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
