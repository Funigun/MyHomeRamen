using MyHomeRamen.Blazor.Features.Menu.Categories.Shared;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;
using MyHomeRamen.Blazor.Features.Menu.Products.Shared;
using MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Features.Menu;

internal static class DependencyInjection
{
    internal static IServiceCollection AddMenuFeatureServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddHttpClient<IngredientApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<CategoryApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<ProductsApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        return services;
    }
}
