using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider, IConfiguration configuration)
    {
        services.AddIdentityPersistance(configurationProvider);
        services.AddKeycloakAdminService(configuration);
        return services;
    }
}
