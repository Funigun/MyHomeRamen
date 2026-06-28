using MyHomeRamen.Common.Contracts.Payments;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Payments.Services;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.Payments;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddPaymentsPersistance(configurationProvider);
        services.AddScoped<IPaymentService, PaymentService>();
        return services;
    }
}

