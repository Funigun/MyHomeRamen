using System.Security.Claims;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Offers.CreateOffer;

namespace MyHomeRamen.UnitTests.MenuModule.Offers;

public sealed class OfferAuthorizationTests
{
    [Fact]
    public async Task CreateOffer_ShouldDenyManager_WhenRequestContextUsesDifferentRestaurant()
    {
        Guid grantedRestaurantId = Guid.CreateVersion7();
        Guid requestedRestaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = new(
        [
            new PermissionGrant("Menu", MenuPermissionConstants.CanManageOffers, true, grantedRestaurantId),
            new PermissionGrant("Menu", MenuPermissionConstants.CanAddOffer, true, grantedRestaurantId)
        ]);
        CompanyContext companyContext = CreateCompanyContext(requestedRestaurantId);
        CreateOfferAuthorizationPolicy policy = new(currentUser, companyContext);

        bool authorized = await policy.Authorize(
            new CreateOfferCommand(Guid.CreateVersion7(), 10m),
            CancellationToken.None);

        Assert.False(authorized);
    }

    [Fact]
    public async Task CreateOffer_ShouldAllowManager_WhenRestaurantGrantMatchesContext()
    {
        Guid restaurantId = Guid.CreateVersion7();
        TestCurrentUser currentUser = new(
        [
            new PermissionGrant("Menu", MenuPermissionConstants.CanManageOffers, true, restaurantId),
            new PermissionGrant("Menu", MenuPermissionConstants.CanAddOffer, true, restaurantId)
        ]);
        CompanyContext companyContext = CreateCompanyContext(restaurantId);
        CreateOfferAuthorizationPolicy policy = new(currentUser, companyContext);

        bool authorized = await policy.Authorize(
            new CreateOfferCommand(Guid.CreateVersion7(), 10m),
            CancellationToken.None);

        Assert.True(authorized);
    }

    private static CompanyContext CreateCompanyContext(Guid restaurantId)
    {
        CompanyContext companyContext = new();
        companyContext.SetRestaurant(restaurantId, Guid.CreateVersion7());
        return companyContext;
    }

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
