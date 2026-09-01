namespace MyHomeRamen.Domain.Identity.Permissions;

public static class PermissionConstants
{
    public const string CanRegisterEmployee = "Employee.Register";
    public const string CanViewEmployee = "Employee.View";
    public const string CanUpdateEmployee = "Employee.Update";
    public const string CanDeleteEmployee = "Employee.Delete";

    public const string CanCreateRole = "Role.Create";
    public const string CanViewRole = "Role.View";
    public const string CanUpdateRole = "Role.Update";
    public const string CanDeleteRole = "Role.Delete";

    public const string CanViewUserProfile = "UserProfile.View";
    public const string CanUpdateUserProfile = "UserProfile.Update";
    public const string CanDeleteUserProfile = "UserProfile.Delete";

    public static readonly string[] AvailablePermissions =
    [
        CanViewUserProfile,
        CanUpdateUserProfile,
        CanDeleteUserProfile,

        CanCreateRole,
        CanViewRole,
        CanUpdateRole,
        CanDeleteRole,

        CanRegisterEmployee,
        CanViewEmployee,
        CanUpdateEmployee,
        CanDeleteEmployee
    ];
}
