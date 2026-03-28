using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Menu;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddMenuPersistance(configurationProvider);
        return services;
    }
}
