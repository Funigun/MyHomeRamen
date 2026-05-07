using MyHomeRamen.Blazor.Features.Account.Common.Services;
using MyHomeRamen.Blazor.Features.Admin.Employees;
using MyHomeRamen.Blazor.Features.Menu.Common.Services;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;
using MyHomeRamen.Blazor.Presentation.Authentication;
using MyHomeRamen.Blazor.Presentation.GuestAuthentication;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Presentation;

internal static class ApiDependencyInjection
{
    internal static IServiceCollection AddApiServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddTransient<GuestCookieForwardingHandler>();
        services.AddScoped<GuestSessionService>();

        services.AddHttpClient<CustomerAccountApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.IdentityApi(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<EmployeeApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.IdentityApi(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AdminAuthHeaderHandler>();

        services.AddHttpClient<MenuApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<ShoppingCartApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>()
         .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        return services;
    }
}
