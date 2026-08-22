namespace MyHomeRamen.Domain.Identity.Roles;

public static class RoleConstants
{
    public const string Admin = "Admin";
    public const string Guest = "Guest";
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles => [Admin, Employee, Manager, Customer];
}
