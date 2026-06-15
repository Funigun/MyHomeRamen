using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Payments.Services;
using MyHomeRamen.Common.Contracts.Payments;
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
