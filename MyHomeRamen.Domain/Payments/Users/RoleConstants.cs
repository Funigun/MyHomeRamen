namespace MyHomeRamen.Domain.Payments.Users;

public static class RoleConstants
{
    public const string Admin = "PaymentAdmin";

    public const string Employee = "Employee";

    public const string Waiter = "Waiter";

    public const string Chef = "Chef";

    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles =>
     [
        Admin,
        Employee,
        Waiter,
        Chef,
        Customer
     ];
}
