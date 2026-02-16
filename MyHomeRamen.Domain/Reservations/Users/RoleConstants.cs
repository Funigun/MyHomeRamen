namespace MyHomeRamen.Domain.Reservations.Users;

public static class RoleConstants
{
    public const string Admin = "ReservationsAdmin";

    public const string Employee = "Employee";

    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles =>
     [
        Admin,
        Employee,
        Customer
     ];
}
