using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Domain.Menu.Roles;

namespace MyHomeRamen.Domain.Menu.Users;

internal static class UserValidator
{
    internal static void Validate(User user)
    {
        CheckRoles(user);
        CheckPermissions(user);
    }

    private static void CheckRoles(User user)
    {
        if (user.Roles.Count == 0)
        {
            throw UserErrors.MissingRole();
        }

        if (user.Roles.Any(r => !RoleConstants.AvailableRoles.Contains(r.Name)))
        {
            throw UserErrors.InvalidRoleName();
        }
    }

    private static void CheckPermissions(User user)
    {
        if (user.Permissions.Count == 0)
        {
            throw UserErrors.MissingPermission();
        }

        if (user.Permissions.Any(r => !PermissionConstants.AvailablePermissions.Contains(r.Name)))
        {
            throw UserErrors.InvalidPermissionName();
        }
    }
}
