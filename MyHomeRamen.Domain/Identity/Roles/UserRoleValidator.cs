namespace MyHomeRamen.Domain.Identity.Roles;

internal static class UserRoleValidator
{
    internal static void ValidateUserRole(UserRole userRole)
    {
        if (userRole.UserId.Value == Guid.Empty)
        {
            throw UserRoleErrors.UserRequired();
        }

        if (userRole.RoleId.Value == Guid.Empty)
        {
            throw UserRoleErrors.RoleRequired();
        }
    }
}
