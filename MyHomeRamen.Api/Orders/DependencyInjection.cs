using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Orders;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
    {
        services.AddOrdersPersistance(configurationProvider);
        return services;
    }
}
