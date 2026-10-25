using MyHomeRamen.Blazor.Features.Account.Common.Services;
using MyHomeRamen.Blazor.Features.Admin.Employees;
using MyHomeRamen.Blazor.Features.Restaurants;
using MyHomeRamen.Blazor.Features.Menu.Common.Services;
using MyHomeRamen.Blazor.Features.Menu;
using MyHomeRamen.Blazor.Features.Payments.Common.Services;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;
using MyHomeRamen.Blazor.Features.ShoppingCart;
using MyHomeRamen.Blazor.Infrastructure.Api;
using MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication;

internal static class ApiDependencyInjection
{
    internal static IServiceCollection AddApiServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddTransient<GuestCookieForwardingHandler>();
        services.AddScoped<GuestSessionService>();
        services.AddScoped<ApiService>();

        services.AddHttpClient<CustomerAccountApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddRestaurantsFeatureServices(infrastructurePrefix);
        services.AddMenuFeatureServices(infrastructurePrefix);
        services.AddShoppingCartFeatureServices(infrastructurePrefix);

        services.AddHttpClient<EmployeeApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AdminAuthHeaderHandler>();

        services.AddHttpClient<MenuApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<PaymentApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        return services;
    }
}
