using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Menu.Services;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Menu;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddMenuPersistance(configurationProvider);
        services.AddScoped<IMenuService, MenuService>();
        return services;
    }
}
