using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.ShoppingCart;

public static class DependencyInjection
{
    public static IServiceCollection AddShoppingCartModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddBasketPersistance(configurationProvider);
        return services;
    }
}
