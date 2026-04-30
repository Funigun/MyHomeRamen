namespace MyHomeRamen.Domain.Common.User;

public static class UserErrors
{
    public static DomainException FirstNameTooLong()
        => new($"First name cannot be longer than {UserConstants.MaxFirstNameLength} characters");

    public static DomainException FirstNameRequired()
        => new("First name is required");

    public static DomainException LastNameTooLong()
        => new($"Last name cannot be longer than {UserConstants.MaxLastNameLength} characters");

    public static DomainException LastNameRequired()
        => new("Last name is required");

    public static DomainException EmailTooLong()
        => new($"Email cannot be longer than {UserConstants.MaxEmailLength} characters");

    public static DomainException EmailRequired()
        => new("Email is required");

    public static DomainException PhoneNumberTooLong()
        => new($"Phone number cannot be longer than {UserConstants.MaxPhoneNumberLength} characters");

    public static DomainException PhoneNumberRequired()
        => new("Phone number is required");

    public static DomainException MissingRole()
        => new($"User must be assigned with at least one role");

    public static DomainException InvalidRoleName()
        => new("Invalid role name");

    public static DomainException MissingPermission()
    => new($"User must be assigned with at least one permission");

    public static DomainException InvalidPermissionName()
        => new("Invalid permission name");

    public static DomainException InvalidIdentity()
        => new("A user must have either a KeycloakUserId or a GuestId, not both and not neither.");
}
