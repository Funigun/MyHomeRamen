using System.Security.Claims;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.UnitTests.IdentityModule.Permissions;

public sealed class PermissionGrantAuthorizationTests
{
    [Fact]
    public void HasPermission_ShouldAllowMatchingRestaurant_WhenGrantIsRestaurantScoped()
    {
        Guid restaurantId = Guid.NewGuid();
        ICurrentUser currentUser = CreateCurrentUser(
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Restaurant, restaurantId));

        Assert.True(currentUser.HasPermission("Restaurants", "Restaurant.Manage", restaurantId));
    }

    [Fact]
    public void HasPermission_ShouldDenyDifferentRestaurant_WhenGrantIsRestaurantScoped()
    {
        ICurrentUser currentUser = CreateCurrentUser(
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Restaurant, Guid.NewGuid()));

        Assert.False(currentUser.HasPermission("Restaurants", "Restaurant.Manage", Guid.NewGuid()));
    }

    [Fact]
    public void HasPermission_ShouldAllowAnyRestaurant_WhenGrantIsCompanyScoped()
    {
        ICurrentUser currentUser = CreateCurrentUser(
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Company, null));

        Assert.True(currentUser.HasPermission("Restaurants", "Restaurant.Manage", Guid.NewGuid()));
    }

    [Fact]
    public void HasPermission_ShouldDenyRestaurantRoleGrant_WhenPermissionIsNotResourceScoped()
    {
        ICurrentUser currentUser = CreateCurrentUser(
            new PermissionGrant("Restaurants", "Restaurants.Create", false, RoleScope.Restaurant, Guid.NewGuid()));

        Assert.False(currentUser.HasPermission("Restaurants", "Restaurants.Create"));
    }

    [Fact]
    public void RestaurantIdsFor_ShouldReturnOnlyAssignedRestaurantIds_WhenGrantsIncludeGlobalAndScopedRoles()
    {
        Guid assignedRestaurantId = Guid.NewGuid();
        ICurrentUser currentUser = CreateCurrentUser(
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Restaurant, assignedRestaurantId),
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Company, null));

        Assert.Equal(new[] { assignedRestaurantId }, currentUser.RestaurantIdsFor("Restaurants", "Restaurant.Manage"));
    }

    private static ICurrentUser CreateCurrentUser(params PermissionGrant[] permissionGrants)
        => new TestCurrentUser(permissionGrants);

    private sealed record TestCurrentUser(IReadOnlyCollection<PermissionGrant> PermissionGrants) : ICurrentUser
    {
        public string IdentityId { get; } = "test-user";
        public Guid UserId { get; } = Guid.NewGuid();
        public IEnumerable<Claim> Claims { get; } = [];
        public bool IsAuthenticated { get; } = true;
        public bool IsGuest { get; } = false;
        public IReadOnlyCollection<string> Permissions { get; } = [];
    }
}
