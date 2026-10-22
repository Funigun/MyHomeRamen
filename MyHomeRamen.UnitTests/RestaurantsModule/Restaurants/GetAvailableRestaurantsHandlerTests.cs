using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetAvailableRestaurants;
using NSubstitute;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Restaurants;

public sealed class GetAvailableRestaurantsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterByAssignedRestaurants_WhenPermissionIsRestaurantScoped()
    {
        Guid restaurantId = Guid.NewGuid();
        (IRestaurantDbContext dbContext, IRestaurantQuery restaurantQuery) = CreateDbContext();
        ICurrentUser currentUser = new TestCurrentUser(
            [new PermissionGrant("Restaurants", "Restaurant.Manage", true, restaurantId)]);
        GetAvailableRestaurantsHandler handler = new(dbContext, currentUser);

        await handler.Handle(new GetAvailableRestaurantsQuery(), CancellationToken.None);

        await restaurantQuery.Received(1).Available(
            Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(restaurantId)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldIncludeAllRestaurants_WhenPermissionIsGlobal()
    {
        (IRestaurantDbContext dbContext, IRestaurantQuery restaurantQuery) = CreateDbContext();
        ICurrentUser currentUser = new TestCurrentUser(
            [new PermissionGrant("Restaurants", "Restaurant.Manage", true, null)]);
        GetAvailableRestaurantsHandler handler = new(dbContext, currentUser);

        await handler.Handle(new GetAvailableRestaurantsQuery(), CancellationToken.None);

        await restaurantQuery.Received(1).Available(null, CancellationToken.None);
    }

    private static (IRestaurantDbContext DbContext, IRestaurantQuery RestaurantQuery) CreateDbContext()
    {
        IRestaurantDbContext dbContext = Substitute.For<IRestaurantDbContext>();
        IRestaurantRepository restaurantRepository = Substitute.For<IRestaurantRepository>();
        IRestaurantQuery restaurantQuery = Substitute.For<IRestaurantQuery>();

        dbContext.Restaurant.Returns(restaurantRepository);
        restaurantRepository.Query().Returns(restaurantQuery);
        restaurantQuery.Available(Arg.Any<IReadOnlyCollection<Guid>?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<RestaurantListItemDto>>([]));

        return (dbContext, restaurantQuery);
    }

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
