using MyHomeRamen.Domain.Common.User;

namespace MyHomeRamen.Domain.Reservations.Users;

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
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            throw UserErrors.FirstNameRequired();
        }

        if (user.FirstName.Length > UserConstants.MaxFirstNameLength)
        {
            throw UserErrors.FirstNameTooLong();
        }

        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            throw UserErrors.LastNameRequired();
        }

        if (user.LastName.Length > UserConstants.MaxLastNameLength)
        {
            throw UserErrors.LastNameTooLong();
        }
    }

    private static void CheckEmail(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw UserErrors.EmailRequired();
        }

        if (user.Email.Length > UserConstants.MaxEmailLength)
        {
            throw UserErrors.EmailTooLong();
        }
    }

    private static void CheckPhoneNumber(User user)
    {
        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            throw UserErrors.PhoneNumberRequired();
        }

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
