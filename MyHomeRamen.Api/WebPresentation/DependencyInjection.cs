using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Features.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider, IConfiguration configuration)
    {
        services.AddIdentityPersistance(configurationProvider);
        services.AddKeycloakAdminService(configuration);
        return services;
    }
}

