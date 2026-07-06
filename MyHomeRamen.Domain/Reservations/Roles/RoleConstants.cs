using MyHomeRamen.Domain.Reservations.Permissions;

namespace MyHomeRamen.Domain.Reservations.Roles;

public static class RoleConstants
{
    public const string Admin = "ReservationAdmin";

    public const string Employee = "ReservationEmployee";

    public const string Waiter = "ReservationWaiter";

    public const string Chef = "ReservationChef";

    public const string Customer = "ReservationCustomer";

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
