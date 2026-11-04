using System.Security.Claims;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.MenuCards.ActivateMenuCard;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;

namespace MyHomeRamen.UnitTests.MenuModule;

public sealed class RestaurantManagementAuthorizationTests
{
    [Fact]
    public async Task GetIngredientPricesForManage_ShouldDeny_WhenManagePermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanViewIngredientPrice);
        GetRestaurantIngredientPricesForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetRestaurantIngredientPricesForManageQuery(
                new PageParameters(),
                new GetRestaurantIngredientPricesForManageRequest(null, null, null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetIngredientPricesForManage_ShouldDeny_WhenViewPermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanManageIngredientPrices);
        GetRestaurantIngredientPricesForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetRestaurantIngredientPricesForManageQuery(
                new PageParameters(),
                new GetRestaurantIngredientPricesForManageRequest(null, null, null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetOffersForManage_ShouldDeny_WhenEitherPermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanManageOffers);
        GetRestaurantOffersForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetRestaurantOffersForManageQuery(
                new PageParameters(),
                new GetRestaurantOffersForManageRequest(
                    null,
                    null,
                    null,
                    null,
                    null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetOffersForManage_ShouldDeny_WhenManagePermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanViewOffer);
        GetRestaurantOffersForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetRestaurantOffersForManageQuery(
                new PageParameters(),
                new GetRestaurantOffersForManageRequest(null, null, null, null, null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetMenuCardsForManage_ShouldDeny_WhenViewPermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanManageMenuCards);
        GetMenuCardsForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetMenuCardsForManageQuery(
                new PageParameters(),
                new GetMenuCardsForManageRequest(null, null, null, null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetMenuCardsForManage_ShouldDeny_WhenManagePermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanViewMenuCard);
        GetMenuCardsForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new GetMenuCardsForManageQuery(
                new PageParameters(),
                new GetMenuCardsForManageRequest(null, null, null, null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task ActivateMenuCard_ShouldDeny_WhenEditPermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanManageMenuCards);
        ActivateMenuCardAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new ActivateMenuCardCommand(Guid.CreateVersion7()),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task ActivateMenuCard_ShouldDeny_WhenManagePermissionIsMissing()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(restaurantId, MenuPermissionConstants.CanEditMenuCard);
        ActivateMenuCardAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(restaurantId));

        bool authorized = await policy.Authorize(
            new ActivateMenuCardCommand(Guid.CreateVersion7()),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task GetOffersForManage_ShouldDeny_WhenPermissionGrantBelongsToAnotherRestaurant()
    {
        Guid grantedRestaurantId = Guid.CreateVersion7();
        Guid selectedRestaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = CreateUser(
            grantedRestaurantId,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanViewOffer);
        GetRestaurantOffersForManageAuthorizationPolicy policy = new(
            currentUser,
            CreateCompanyContext(selectedRestaurantId));

        bool authorized = await policy.Authorize(
            new GetRestaurantOffersForManageQuery(
                new PageParameters(),
                new GetRestaurantOffersForManageRequest(
                    null,
                    null,
                    RestaurantProductOfferStatus.Active,
                    null,
                    null)),
            CancellationToken.None);

        Assert.False(authorized);
    }

    private static CompanyContext CreateCompanyContext(Guid restaurantId)
    {
        CompanyContext companyContext = new();
        companyContext.SetRestaurant(restaurantId, Guid.CreateVersion7());
        return companyContext;
    }

    private static TestCurrentUser CreateUser(Guid restaurantId, params string[] permissions)
        => new(permissions
            .Select(permission => new PermissionGrant("Menu", permission, true, RoleScope.Restaurant, restaurantId))
            .ToArray());

    private sealed class TestCurrentUser(IReadOnlyCollection<PermissionGrant> permissionGrants) : ICurrentUser
    {
        public string IdentityId => "test-user";

        public Guid UserId => Guid.CreateVersion7();

        public IEnumerable<Claim> Claims => [];

        public bool IsAuthenticated => true;

        public bool IsGuest => false;

        public IReadOnlyCollection<string> Permissions => [];

        public IReadOnlyCollection<PermissionGrant> PermissionGrants => permissionGrants;
    }
}
