using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Infrastructure.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

public static class KeycloakAdminExtensions
{
    public static IServiceCollection AddKeycloakAdminService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakAdminOptions>(options =>
        {
            configuration.GetSection("KeycloakAdmin").Bind(options);

            // Aspire injects: services:my-home-ramen-key-cloak:http:0 = http://localhost:{dynamic-port}
            // Override BaseUrl so the admin token handler uses the correct dynamic endpoint
            string? dynamicUrl =
                configuration["services:my-home-ramen-key-cloak:https:0"] ??
                configuration.GetConnectionString("my-home-ramen-key-cloak") ??
                configuration["Authorization:BaseUrl"];

            if (!string.IsNullOrEmpty(dynamicUrl))
            {
                options.BaseUrl = dynamicUrl.TrimEnd('/');
                options.ClientSecret = configuration["Authentication:KeycloakAdmin:ClientSecret"]!;
                options.ClientId = configuration["Authentication:KeycloakAdmin:ClientId"]!;
                options.Realm = configuration["Authorization:Realm"]!;
            }
        });

        services.AddCacheService();
        services.AddTransient<KeycloakAdminTokenHandler>();

        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>()
                .AddHttpMessageHandler<KeycloakAdminTokenHandler>();

        return services;
    }
}
