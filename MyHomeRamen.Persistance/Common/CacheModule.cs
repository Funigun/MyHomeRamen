using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Persistance.Cache;

internal sealed class MenuCacheModule : ICacheModule
{
    public static string ModuleName => "Menu";
}

internal sealed class OrderCacheModule : ICacheModule
{
    public static string ModuleName => "Order";
}

internal sealed class IdentityCacheModule : ICacheModule
{
    public static string ModuleName => "User";
}

internal sealed class PaymentCacheModule : ICacheModule
{
    public static string ModuleName => "Payment";
}

internal sealed class ShoppingCartCacheModule : ICacheModule
{
    public static string ModuleName => "ShoppingCart";
}

internal sealed class ReservationCacheModule : ICacheModule
{
    public static string ModuleName => "Reservation";
}

