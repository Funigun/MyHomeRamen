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

    public static Dictionary<string, IEnumerable<string>> DefaultPermissions => new()
    {
        { Admin, PermissionConstants.AvailablePermissions },
        { Employee, PermissionConstants.AvailablePermissions },
        {
            Waiter,
            [PermissionConstants.CanViewBookingsManagementView,
                  PermissionConstants.CanAddBooking,
                  PermissionConstants.CanEditBooking,
                  PermissionConstants.CanCancelBooking,
                  PermissionConstants.CanViewCustomerBookings,
                  PermissionConstants.CanViewTablesManagementView]
        },
        {
            Chef,
            [PermissionConstants.CanViewCustomerBookings]
        },
        { Customer, [PermissionConstants.CanAddBooking, PermissionConstants.CanEditBooking, PermissionConstants.CanCancelBooking, PermissionConstants.CanViewBookingsHistory] }
    };
}
