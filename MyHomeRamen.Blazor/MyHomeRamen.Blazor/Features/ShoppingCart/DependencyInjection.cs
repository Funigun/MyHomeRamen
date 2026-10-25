using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;
using MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Features.ShoppingCart;

internal static class DependencyInjection
{
    internal static IServiceCollection AddShoppingCartFeatureServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddHttpClient<ShoppingCartApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        return services;
    }
}
