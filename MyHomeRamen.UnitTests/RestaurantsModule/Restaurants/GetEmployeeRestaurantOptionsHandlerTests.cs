using System.Security.Claims;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetEmployeeRestaurantOptions;
using NSubstitute;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Restaurants;

public sealed class GetEmployeeRestaurantOptionsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldQueryDistinctRestaurantIds_WhenUserHasMultipleRestaurantScopedGrants()
    {
        Guid firstRestaurantId = Guid.NewGuid();
        Guid secondRestaurantId = Guid.NewGuid();
        (IRestaurantDbContext dbContext, IRestaurantQuery restaurantQuery) = CreateDbContext();
        EmployeeRestaurantOptionDto[] expectedRestaurants =
        [
            new(firstRestaurantId, "First", "Street 1", "Warsaw", "00-001", 52.1, 21.1),
            new(secondRestaurantId, "Second", "Street 2", "Warsaw", "00-002", 52.2, 21.2)
        ];
        restaurantQuery.EmployeeOptions(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<EmployeeRestaurantOptionDto>>(expectedRestaurants));
        ICurrentUser currentUser = CreateCurrentUser(
        [
            new PermissionGrant("Menu", "Menu.View", true, RoleScope.Restaurant, firstRestaurantId),
            new PermissionGrant("Menu", "Menu.Edit", true, RoleScope.Restaurant, firstRestaurantId),
            new PermissionGrant("Restaurants", "Restaurant.Details.Edit", true, RoleScope.Restaurant, secondRestaurantId),
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Company, null)
        ]);
        GetEmployeeRestaurantOptionsHandler handler = new(dbContext, currentUser);

        GetEmployeeRestaurantOptionsResponse response = await handler.Handle(
            new GetEmployeeRestaurantOptionsQuery(),
            CancellationToken.None);

        Assert.Equal(expectedRestaurants, response.Restaurants);
        await restaurantQuery.Received(1).EmployeeOptions(
            Arg.Is<IReadOnlyCollection<Guid>>(ids =>
                ids.Count == 2
                && ids.Contains(firstRestaurantId)
                && ids.Contains(secondRestaurantId)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Authorize_ShouldAllowAnyResourceScopedRestaurantPermission()
    {
        Guid restaurantId = Guid.NewGuid();
        ICurrentUser currentUser = CreateCurrentUser(
        [
            new PermissionGrant("Menu", "Menu.View", true, RoleScope.Restaurant, restaurantId)
        ]);
        GetEmployeeRestaurantOptionsAuthorizationPolicy policy = new(currentUser);

        bool isAuthorized = await policy.Authorize(new GetEmployeeRestaurantOptionsQuery(), CancellationToken.None);

        Assert.True(isAuthorized);
    }

    [Fact]
    public async Task Authorize_ShouldRejectCompanyScopedGrants()
    {
        ICurrentUser currentUser = CreateCurrentUser(
        [
            new PermissionGrant("Restaurants", "Restaurant.Manage", true, RoleScope.Company, null)
        ]);
        GetEmployeeRestaurantOptionsAuthorizationPolicy policy = new(currentUser);

        bool isAuthorized = await policy.Authorize(new GetEmployeeRestaurantOptionsQuery(), CancellationToken.None);

        Assert.False(isAuthorized);
    }

    [Fact]
    public async Task Authorize_ShouldRejectRestaurantGrantWithoutResourceScopeOrRestaurantId()
    {
        ICurrentUser currentUser = CreateCurrentUser(
        [
            new PermissionGrant("Menu", "Menu.View", false, RoleScope.Restaurant, Guid.NewGuid()),
            new PermissionGrant("Menu", "Menu.Edit", true, RoleScope.Restaurant, null)
        ]);
        GetEmployeeRestaurantOptionsAuthorizationPolicy policy = new(currentUser);

        bool isAuthorized = await policy.Authorize(new GetEmployeeRestaurantOptionsQuery(), CancellationToken.None);

        Assert.False(isAuthorized);
    }

    [Fact]
    public async Task Authorize_ShouldRejectUnauthenticatedUser_WhenRestaurantGrantExists()
    {
        ICurrentUser currentUser = CreateCurrentUser(
        [
            new PermissionGrant("Menu", "Menu.View", true, RoleScope.Restaurant, Guid.NewGuid())
        ],
        isAuthenticated: false);
        GetEmployeeRestaurantOptionsAuthorizationPolicy policy = new(currentUser);

        bool isAuthorized = await policy.Authorize(new GetEmployeeRestaurantOptionsQuery(), CancellationToken.None);

        Assert.False(isAuthorized);
    }

    private static (IRestaurantDbContext DbContext, IRestaurantQuery RestaurantQuery) CreateDbContext()
    {
        IRestaurantDbContext dbContext = Substitute.For<IRestaurantDbContext>();
        IRestaurantRepository restaurantRepository = Substitute.For<IRestaurantRepository>();
        IRestaurantQuery restaurantQuery = Substitute.For<IRestaurantQuery>();

        dbContext.Restaurant.Returns(restaurantRepository);
        restaurantRepository.Query().Returns(restaurantQuery);
        restaurantQuery.EmployeeOptions(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<EmployeeRestaurantOptionDto>>([]));

        return (dbContext, restaurantQuery);
    }

    private static ICurrentUser CreateCurrentUser(
        IReadOnlyCollection<PermissionGrant> permissionGrants,
        bool isAuthenticated = true)
        => new TestCurrentUser(permissionGrants, isAuthenticated);

    private sealed record TestCurrentUser(
        IReadOnlyCollection<PermissionGrant> PermissionGrants,
        bool IsAuthenticated) : ICurrentUser
    {
        public string IdentityId { get; } = "test-user";
        public Guid UserId { get; } = Guid.NewGuid();
        public IEnumerable<Claim> Claims { get; } = [];
        public bool IsGuest { get; } = false;
        public IReadOnlyCollection<string> Permissions { get; } = [];
    }
}
