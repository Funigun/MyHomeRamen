namespace MyHomeRamen.Domain.Payments.Users;

public static class PermissionConstants
{
    public const string CanViewPayments = "CanViewPayments";
    public const string CanPay = "CanPay";
    public const string CanManageRefunds = "CanManageRefunds";

    public static IEnumerable<string> AvailablePermissions =>
     [
        CanViewPayments,
        CanPay,
        CanManageRefunds
     ];
}
