namespace MyHomeRamen.Domain.Orders.Users;

public static class PermissionConstants
{
    public const string CanAcceptOrder = "CanAcceptOrder";
    public const string CanRejectOrder = "CanRejectOrder";
    public const string CanCancelOrder = "CanCancelOrder";
    public const string CanMarkAsComplete = "CanMarkAsComplete";
    public const string CanMarkAsPrepared = "CanMarkAsPrepared";
    public const string CanShowOrdersHistory = "CanShowOrdersHistory";
    public const string CanViewCustomerOrders = "CanViewCustomerOrders";

    public const string CanCancelPayment = "CanCancelPayment";
    public const string CanSplitPayment = "CanSplitPayment";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CanAcceptOrder,
        CanRejectOrder,
        CanCancelOrder,
        CanMarkAsComplete,
        CanMarkAsPrepared,
        CanShowOrdersHistory,
        CanViewCustomerOrders,
        CanCancelPayment,
        CanSplitPayment
    ];
}
