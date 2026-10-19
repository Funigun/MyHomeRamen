using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Identity.Roles;

public static class UserRoleErrors
{
    public static DomainException UserRequired()
        => new("User role user is required.");

    public static DomainException RoleRequired()
        => new("User role role is required.");

    public static DomainException RoleAlreadyAssigned()
        => new("Role is already assigned to the user.");
}
