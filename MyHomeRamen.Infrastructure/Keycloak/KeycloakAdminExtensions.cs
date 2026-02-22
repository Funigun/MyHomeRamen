using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Infrastructure.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

public static class KeycloakAdminExtensions
{
    public static IServiceCollection AddKeycloakAdminService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakAdminOptions>(configuration.GetSection("KeycloakAdmin"));
        services.AddTransient<KeycloakAdminTokenHandler>();

        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>()
                .AddHttpMessageHandler<KeycloakAdminTokenHandler>();

        return services;
    }
}
