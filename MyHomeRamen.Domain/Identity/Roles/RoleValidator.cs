namespace MyHomeRamen.Domain.Identity.Roles;

internal static class RoleValidator
{
    internal static void ValidateRole(Role role)
    {
        if (!Enum.IsDefined(role.Scope))
        {
            throw RoleErrors.ScopeInvalid();
        }

        if (role.RestaurantId == Guid.Empty)
        {
            throw RoleErrors.RestaurantIdCannotBeEmpty();
        }

        if (role.Scope == RoleScope.Company && role.RestaurantId.HasValue)
        {
            throw RoleErrors.RestaurantIdNotAllowedForCompanyScope();
        }

        if (role.Scope == RoleScope.Restaurant && !role.RestaurantId.HasValue)
        {
            throw RoleErrors.RestaurantIdRequiredForRestaurantScope();
        }
    }
}
