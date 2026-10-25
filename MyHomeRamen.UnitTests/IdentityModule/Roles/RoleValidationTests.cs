using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.UnitTests.IdentityModule.Roles;

public sealed class RoleValidationTests
{
    [Fact]
    public void CreateCustom_ShouldCreateGlobalRole_WhenRestaurantIdIsNull()
    {
        Role role = Role.CreateCustom("Customer", "Customer role");

        Assert.Null(role.RestaurantId);
    }

    [Fact]
    public void CreateCustom_ShouldStoreRestaurantId_WhenProvided()
    {
        Guid restaurantId = Guid.CreateVersion7();

        Role role = Role.CreateCustom("Restaurant Admin", "Restaurant role", restaurantId);

        Assert.Equal(restaurantId, role.RestaurantId);
    }

    [Fact]
    public void CreateCustom_ShouldRejectEmptyRestaurantId_WhenProvided()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            Role.CreateCustom("Restaurant Admin", "Restaurant role", Guid.Empty));

        Assert.Equal(RoleErrors.RestaurantIdCannotBeEmpty().Message, exception.Message);
    }
}
