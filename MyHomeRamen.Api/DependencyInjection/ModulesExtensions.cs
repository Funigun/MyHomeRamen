using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Menu.Services;
using MyHomeRamen.Features.Payments.ExternalApi;
using MyHomeRamen.Features.Payments.Services;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Persistance;

namespace MyHomeRamen.Api.DependencyInjection;

internal static class ModulesExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddMenuModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddMenuPersistance(configurationProvider);
            services.AddScoped<IMenuService, MenuService>();

            return services;
        }

        internal IServiceCollection AddShoppingCartModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddBasketPersistance(configurationProvider);
            return services;
        }

        internal IServiceCollection AddOrdersModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddOrdersPersistance(configurationProvider);
            return services;
        }

        internal IServiceCollection AddPaymentsModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddPaymentsPersistance(configurationProvider);
            services.AddScoped<IPaymentService, PaymentService>();
            
            return services;
        }

        internal IServiceCollection AddReservationsModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddReservationsPersistance(configurationProvider);
            
            return services;
        }

        internal IServiceCollection AddUsersModule(DatabaseConfigurationProvider configurationProvider, IConfiguration configuration)
        {
            services.AddIdentityPersistance(configurationProvider);
            services.AddKeycloakAdminService(configuration);
            
            return services;
        }

        internal IServiceCollection AddRestaurantsModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddRestaurantsPersistance(configurationProvider);

            return services;
        }
    }
}
