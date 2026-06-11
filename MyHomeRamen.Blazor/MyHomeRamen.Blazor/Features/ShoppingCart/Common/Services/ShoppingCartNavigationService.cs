using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;

public sealed class ShoppingCartNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public static class Public
        {
            public static string Checkout() => $"/checkout";
        }
    }

    public void ToCheckout() => navigation.NavigateTo(Routes.Public.Checkout());
}
