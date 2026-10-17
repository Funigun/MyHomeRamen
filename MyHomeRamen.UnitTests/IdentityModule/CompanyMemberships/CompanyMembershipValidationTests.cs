using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.IdentityModule.CompanyMemberships;

public sealed class CompanyMembershipValidationTests
{
    [Fact]
    public void Create_ShouldAllowCompanyWideMembership_WhenRestaurantIdIsNull()
    {
        CompanyMembership membership = CreateMembership();

        Assert.Null(membership.RestaurantId);
    }

    [Fact]
    public void Create_ShouldStoreRestaurantId_WhenProvided()
    {
        Guid restaurantId = Guid.CreateVersion7();

        CompanyMembership membership = CreateMembership(restaurantId);

        Assert.Equal(restaurantId, membership.RestaurantId);
    }

    [Fact]
    public void Create_ShouldRejectEmptyRestaurantId_WhenProvided()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            CompanyMembership.Create(new UserId(Guid.CreateVersion7()), Guid.CreateVersion7(), Guid.Empty));

        Assert.Equal(CompanyMembershipErrors.RestaurantIdCannotBeEmpty().Message, exception.Message);
    }

    private static CompanyMembership CreateMembership(Guid? restaurantId = null)
        => CompanyMembership.Create(new UserId(Guid.CreateVersion7()), Guid.CreateVersion7(), restaurantId);
}
