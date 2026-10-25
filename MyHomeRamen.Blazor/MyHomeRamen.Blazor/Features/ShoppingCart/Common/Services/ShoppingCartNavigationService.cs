using Microsoft.AspNetCore.Components;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Shared;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;

public sealed class ShoppingCartNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string Checkout = BasketRoutes.Checkout;
    }

    public void ToCheckout() => navigation.NavigateTo(Routes.Checkout);
}
