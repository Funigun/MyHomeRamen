using MyHomeRamen.Blazor.Features.Account.Common.Services;
using MyHomeRamen.Blazor.Features.Admin.Common.Services;
using MyHomeRamen.Blazor.Features.Home.Common.Services;
using MyHomeRamen.Blazor.Features.Menu.Common.Services;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;

namespace MyHomeRamen.Blazor.Presentation;

internal static class NavigationDependencyInjection
{
    internal static IServiceCollection AddNavigationServices(this IServiceCollection services)
    {
        services.AddScoped<AccountNavigationService>();
        services.AddScoped<AdminNavigationService>();
        services.AddScoped<MenuNavigationService>();
        services.AddScoped<ShoppingCartNavigationService>();
        services.AddScoped<HomeNavigationService>();

        return services;
    }
}
