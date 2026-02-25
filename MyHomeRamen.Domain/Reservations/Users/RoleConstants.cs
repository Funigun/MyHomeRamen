namespace MyHomeRamen.Domain.Reservations.Users;

public static class RoleConstants
{
    public const string Admin = "ReservationAdmin";

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
