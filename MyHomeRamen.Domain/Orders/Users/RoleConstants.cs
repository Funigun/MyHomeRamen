namespace MyHomeRamen.Domain.Orders.Users;

public static class RoleConstants
{
    public const string Admin = "OrderAdmin";

    public const string Employee = "OrderEmployee";

    public const string Waiter = "OrderWaiter";

    public const string Chef = "OrderChef";

    public const string Customer = "OrderCustomer";

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
        { Employee, PermissionConstants.AvailablePermissions.Where(p => p != PermissionConstants.CanCancelPayment) },
        {
            Waiter,
            [PermissionConstants.CanAcceptOrder,
                  PermissionConstants.CanRejectOrder,
                  PermissionConstants.CanCancelOrder,
                  PermissionConstants.CanMarkAsPrepared,
                  PermissionConstants.CanViewCustomerOrders,
                  PermissionConstants.CanCancelPayment,
                  PermissionConstants.CanSplitPayment]
        },
        {
            Chef,
            [PermissionConstants.CanMarkAsPrepared,
            PermissionConstants.CanMarkAsComplete,
            PermissionConstants.CanViewCustomerOrders]
        },
        { Customer, [PermissionConstants.CanShowOrdersHistory, PermissionConstants.CanCancelOrder, PermissionConstants.CanCancelPayment] }
    };
}
