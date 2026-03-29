using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Payments;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddPaymentsPersistance(configurationProvider);
        return services;
    }
}
