using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.UnitTests.IdentityModule.Roles;

public sealed class RoleValidationTests
{
    [Fact]
    public void CreateCustom_ShouldCreateCompanyRole_WhenCompanyScopeHasNoRestaurantId()
    {
        Role role = Role.CreateCustom("Customer", "Customer role", RoleScope.Company);

        Assert.Equal(RoleScope.Company, role.Scope);
        Assert.Null(role.RestaurantId);
    }

    [Fact]
    public void CreateCustom_ShouldCreateRestaurantRole_WhenRestaurantScopeHasRestaurantId()
    {
        Guid restaurantId = Guid.CreateVersion7();

        Role role = Role.CreateCustom("Restaurant Admin", "Restaurant role", RoleScope.Restaurant, restaurantId);

        Assert.Equal(RoleScope.Restaurant, role.Scope);
        Assert.Equal(restaurantId, role.RestaurantId);
    }

    [Fact]
    public void CreateCustom_ShouldRejectCompanyScopeWithRestaurantId()
    {
        Guid restaurantId = Guid.CreateVersion7();

        DomainException exception = Assert.Throws<DomainException>(() =>
            Role.CreateCustom("Customer", "Customer role", RoleScope.Company, restaurantId));

        Assert.Equal(RoleErrors.RestaurantIdNotAllowedForCompanyScope().Message, exception.Message);
    }

    [Fact]
    public void CreateCustom_ShouldRejectRestaurantScopeWithoutRestaurantId()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            Role.CreateCustom("Restaurant Admin", "Restaurant role", RoleScope.Restaurant));

        Assert.Equal(RoleErrors.RestaurantIdRequiredForRestaurantScope().Message, exception.Message);
    }

    [Fact]
    public void CreateCustom_ShouldRejectEmptyRestaurantId_WhenProvided()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            Role.CreateCustom("Restaurant Admin", "Restaurant role", RoleScope.Restaurant, Guid.Empty));

        Assert.Equal(RoleErrors.RestaurantIdCannotBeEmpty().Message, exception.Message);
    }
}
