namespace MyHomeRamen.Domain.Reservations.Users;

public static class PermissionConstants
{
    public const string CanViewBookingsManagementView = "CanViewBookingsManagementView";
    public const string CanAddBooking = "CanAddBooking";
    public const string CanEditBooking = "CanEditBooking";
    public const string CanCancelBooking = "CanCancelBooking";
    public const string CanViewBookingsHistory = "CanViewBookingsHistory";
    public const string CanViewCustomerBookings = "CanViewCustomerBookings";

    public const string CanViewTablesManagementView = "CanViewTablesManagementView";
    public const string CanAddTable = "CanAddTable";
    public const string CanEditTable = "CanEditTable";
    public const string CanDeleteTable = "CanDeleteTable";

    public static IEnumerable<string> AvailablePermissions =>
     [
        CanViewBookingsManagementView,
        CanAddBooking,
        CanEditBooking,
        CanCancelBooking,
        CanViewBookingsHistory,
        CanViewCustomerBookings,
        CanViewTablesManagementView,
        CanAddTable,
        CanEditTable,
        CanDeleteTable
     ];
}
