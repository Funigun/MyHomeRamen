using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public static class Public
        {
            public static string ProductDetail(Guid id) => $"/menu/products/{id}";
        }
    }

    public void ToProductDetail(Guid id) => navigation.NavigateTo(Routes.Public.ProductDetail(id));
}
