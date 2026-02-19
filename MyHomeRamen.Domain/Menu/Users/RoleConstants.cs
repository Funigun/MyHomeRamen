namespace MyHomeRamen.Domain.Menu.Users;

public static class RoleConstants
{
    public const string Admin = "MenuAdmin";

    public const string Employee = "Employee";

    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles =>
     [
        Admin,
        Employee,
        Customer
     ];
}
