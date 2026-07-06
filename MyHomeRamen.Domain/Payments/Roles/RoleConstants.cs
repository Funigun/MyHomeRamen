using MyHomeRamen.Domain.Payments.Permissions;

namespace MyHomeRamen.Domain.Payments.Roles;

public static class RoleConstants
{
    public const string Admin = "PaymentAdmin";

    public const string Employee = "PaymentEmployee";

    public const string Waiter = "PaymentWaiter";

    public const string Chef = "PaymentChef";

    public const string Customer = "PaymentCustomer";

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
            [PermissionConstants.CanViewPayments,
                  PermissionConstants.CanPay]
        },
        {
            Chef,
            [PermissionConstants.CanViewPayments]
        },
        { Customer, [PermissionConstants.CanPay, PermissionConstants.CanViewPayments] }
    };
}
