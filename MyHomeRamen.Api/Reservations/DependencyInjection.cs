using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Reservations;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddReservationsPersistance(configurationProvider);
        return services;
    }
}
