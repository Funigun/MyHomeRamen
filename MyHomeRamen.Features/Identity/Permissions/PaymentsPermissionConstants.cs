namespace MyHomeRamen.Features.Identity.Permissions;

public static class PaymentsPermissionConstants
{
    public const string CanViewPayments = "CanViewPayments";
    public const string CanPay = "CanPay";
    public const string CanManageRefunds = "CanManageRefunds";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CanViewPayments, CanPay, CanManageRefunds
    ];
}
