namespace MyHomeRamen.Infrastructure.Keycloak.Constants;

public static class KeycloakRoleConstants
{
    public const string Employee = "employee";
    public const string Customer = "customer";
    public const string Manager = "manager";

    public const string MenuCustomer = "menu_customer";
    public const string MenuEmployee = "menu_employee";
    public const string MenuAdmin = "menu_admin";

    internal static IEnumerable<string> AllRoles =>
    [
        Employee, Customer, Manager,
        MenuCustomer, MenuEmployee, MenuAdmin
    ];

    internal static Dictionary<string, IEnumerable<string>> RoleMappings => new()
    {
        [Employee] = [Employee, MenuEmployee],
        [Customer] = [Customer, MenuCustomer],
        [Manager] = [Manager, MenuAdmin]
    };

    internal static IEnumerable<string> CustomerRoles => [Customer, MenuCustomer];
}
