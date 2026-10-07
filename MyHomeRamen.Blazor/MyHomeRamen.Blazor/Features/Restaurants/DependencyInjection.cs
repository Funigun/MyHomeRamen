using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;
using MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Features.Restaurants;

internal static class DependencyInjection
{
    internal static IServiceCollection AddRestaurantsFeatureServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddHttpClient<CompanyApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        services.AddHttpClient<SocialMediaApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<GuestCookieForwardingHandler>();

        return services;
    }
}
