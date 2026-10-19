using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Identity.Roles;

public static class RoleErrors
{
    public static DomainException RestaurantIdCannotBeEmpty()
        => new("Role restaurant identifier cannot be empty.");
}
