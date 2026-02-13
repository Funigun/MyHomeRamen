using MyHomeRamen.Domain.Common.Authorization;

namespace MyHomeRamen.Identity.Api.Application;

public static class AuthenticationConstants
{
    public static IEnumerable<string> Roles { get; } =
    [
        CommonRoleConstants.Customer,
        CommonRoleConstants.Cook,
        CommonRoleConstants.Waiter,
        MyHomeRamen.Domain.Menu.Authorization.RoleConstants.Admin,
        MyHomeRamen.Domain.Orders.Authorization.RoleConstants.OrdersAdmin,
        MyHomeRamen.Domain.Reservations.Authorization.RoleConstants.ReservationsAdmin
    ];

    public static IEnumerable<string> Permissions { get; } =
    [
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanViewProductsManagementView,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanAddProduct,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanEditProduct,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanDeleteProduct,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanEditProductsRecipes,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanViewCategoriesManagementView,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanAddCategory,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanEditCategory,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanDeleteCategory,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanViewIngredientsManagementView,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanAddIngredient,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanEditIngredient,
        MyHomeRamen.Domain.Menu.Authorization.PermissionConstants.CanDeleteIngredient,

        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanAcceptOrder,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanRejectOrder,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanCancelOrder,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanMarkAsComplete,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanMarkAsPrepared,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanShowOrdersHistory,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanViewCustomerOrders,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanCancelPayment,
        MyHomeRamen.Domain.Orders.Authorization.PermissionConstants.CanSplitPayment,

        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanViewBookingsManagementView,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanAddBooking,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanEditBooking,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanCancelBooking,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanViewBookingsHistory,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanViewCustomerBookings,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanViewTablesManagementView,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanAddTable,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanEditTable,
        MyHomeRamen.Domain.Reservations.Authorization.PermissionConstants.CanDeleteTable
    ];
}
