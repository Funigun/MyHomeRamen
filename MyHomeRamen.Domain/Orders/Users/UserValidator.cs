using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Orders.Permissions;
using MyHomeRamen.Domain.Orders.Roles;

namespace MyHomeRamen.Domain.Orders.Users;

internal static class UserValidator
{
    internal static void Validate(User user)
    {
        CheckName(user);
        CheckEmail(user);
        CheckPhoneNumber(user);
        CheckRoles(user);
        CheckPermissions(user);
    }

    private static void CheckName(User user)
    {
        if (user.FirstName.Length > UserConstants.MaxFirstNameLength)
        {
            throw UserErrors.FirstNameTooLong();
        }

        if (user.LastName.Length > UserConstants.MaxLastNameLength)
        {
            throw UserErrors.LastNameTooLong();
        }
    }

    private static void CheckEmail(User user)
    {
        if (user.Email.Length > UserConstants.MaxEmailLength)
        {
            throw UserErrors.EmailTooLong();
        }
    }

    private static void CheckPhoneNumber(User user)
    {
        if (user.PhoneNumber.Length > UserConstants.MaxPhoneNumberLength)
        {
            throw UserErrors.PhoneNumberTooLong();
        }
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
