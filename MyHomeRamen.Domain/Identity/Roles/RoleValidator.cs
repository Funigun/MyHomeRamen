namespace MyHomeRamen.Domain.Identity.Roles;

internal static class RoleValidator
{
    internal static void ValidateRole(Role role)
    {
        if (role.RestaurantId == Guid.Empty)
        {
            throw RoleErrors.RestaurantIdCannotBeEmpty();
        }
    }
}
