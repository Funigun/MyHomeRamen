using FluentValidation;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using NSubstitute;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Permissions;

public sealed class PermissionCatalogTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task Synchronize_ShouldProvisionCompanyRolesAndLeaveRestaurantAdminUnprovisioned()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        IReadOnlyCollection<Permission> permissions =
            (await apiFactory.IdentityDbContext.Permission.Load().All(cancellationToken)).ToArray();
        Role? companyOwnerRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.CompanyOwner, cancellationToken);
        Role? adminRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.RestaurantAdmin, cancellationToken);
        Role? guestRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.Guest, cancellationToken);
        Role? customerRole = await apiFactory.IdentityDbContext.Role.Load()
            .ByName(RoleConstants.Customer, cancellationToken);

        Assert.NotNull(companyOwnerRole);
        Assert.Null(adminRole);
        Assert.NotNull(guestRole);
        Assert.NotNull(customerRole);
        Assert.Equal(RoleScope.Company, companyOwnerRole.Scope);
        Assert.Equal(RoleScope.Company, guestRole.Scope);
        Assert.Equal(RoleScope.Company, customerRole.Scope);
        Assert.Null(companyOwnerRole.RestaurantId);

        string[] actualCompanyOwnerPermissions = companyOwnerRole.RolePermissions
            .Select(rolePermission => permissions.Single(permission => permission.Id == rolePermission.PermissionId))
            .Select(permission => $"{permission.Module}:{permission.Name}")
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();
        string[] expectedCompanyOwnerPermissions = RestaurantsPermissionConstants.CompanyOwnerPermissions
            .Select(permission => $"Restaurants:{permission}")
            .Concat(MenuPermissionConstants.AvailablePermissions.Select(permission => $"Menu:{permission}"))
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(expectedCompanyOwnerPermissions, actualCompanyOwnerPermissions);

        foreach (Role role in new[] { guestRole, customerRole })
        {
            string[] actualPermissions = role.RolePermissions
                .Select(rolePermission => permissions.Single(permission => permission.Id == rolePermission.PermissionId))
                .Select(permission => $"{permission.Module}:{permission.Name}")
                .ToArray();

            foreach (string expectedPermission in MenuPermissionConstants.CustomerViewPermissions)
            {
                Assert.Contains($"Menu:{expectedPermission}", actualPermissions);
            }

            foreach (string expectedPermission in ShoppingCartPermissionConstants.GuestPermissions)
            {
                Assert.Contains($"ShoppingCart:{expectedPermission}", actualPermissions);
            }
        }

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
    public async Task CreateRestaurantAdminRoleAsync_ShouldCreateRestaurantScopedRoleWithOnlyRestaurantPermissions()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        IKeycloakAdminService keycloakAdminService = Substitute.For<IKeycloakAdminService>();
        IValidator<CompanyOwnerDto> companyOwnerValidator = Substitute.For<IValidator<CompanyOwnerDto>>();
        IdentityService identityService = new(
            keycloakAdminService,
            apiFactory.IdentityDbContext,
            companyOwnerValidator);
        Guid restaurantId = Guid.CreateVersion7();

        try
        {
            Guid roleId = await identityService.CreateRestaurantAdminRoleAsync(restaurantId, cancellationToken);
            Guid repeatedRoleId = await identityService.CreateRestaurantAdminRoleAsync(restaurantId, cancellationToken);
            Role? role = await apiFactory.IdentityDbContext.Role.Load()
                .ByName(RoleConstants.RestaurantAdmin, cancellationToken, restaurantId);
            IReadOnlyCollection<Permission> permissions =
                (await apiFactory.IdentityDbContext.Permission.Load().All(cancellationToken)).ToArray();

            Assert.NotNull(role);
            Assert.Equal(roleId, repeatedRoleId);
            Assert.Equal(RoleScope.Restaurant, role.Scope);
            Assert.Equal(restaurantId, role.RestaurantId);

            string[] actualPermissions = role.RolePermissions
                .Select(rolePermission => permissions.Single(permission => permission.Id == rolePermission.PermissionId))
                .Select(permission => $"{permission.Module}:{permission.Name}")
                .OrderBy(permission => permission, StringComparer.Ordinal)
                .ToArray();
            string[] expectedPermissions = RestaurantsPermissionConstants.ResourceScopedPermissions
                .Select(permission => $"Restaurants:{permission}")
                .Concat(MenuPermissionConstants.RestaurantScopedPermissions.Select(permission => $"Menu:{permission}"))
                .OrderBy(permission => permission, StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expectedPermissions, actualPermissions);
        }
        finally
        {
            Role? role = await apiFactory.IdentityDbContext.Role.Load()
                .ByName(RoleConstants.RestaurantAdmin, cancellationToken, restaurantId);
            if (role is not null)
            {
                apiFactory.IdentityDbContext.Role.Delete(role);
                await apiFactory.IdentityDbContext.SaveChangesAsync(cancellationToken);
            }
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
            RoleScope.Restaurant,
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
            Assert.Equal(RoleScope.Restaurant, grant.Scope);
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
