using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Identity.Roles;

public static class RoleErrors
{
    public static DomainException ScopeInvalid()
        => new("Role scope is invalid.");

    public static DomainException RestaurantIdCannotBeEmpty()
        => new("Role restaurant identifier cannot be empty.");

    public static DomainException RestaurantIdNotAllowedForCompanyScope()
        => new("Company-scoped roles cannot have a restaurant identifier.");

    public static DomainException RestaurantIdRequiredForRestaurantScope()
        => new("Restaurant-scoped roles require a restaurant identifier.");
}
