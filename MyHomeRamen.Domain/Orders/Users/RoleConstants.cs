namespace MyHomeRamen.Domain.Orders.Users;

public static class RoleConstants
{
    public const string Customer = "Customer";

    public const string Employee = "Employee";

    public const string OrdersAdmin = "OrdersAdmin";

    public static IEnumerable<string> AvailableRoles =>
     [
        Customer,
        Employee,
        OrdersAdmin
    ];
}
