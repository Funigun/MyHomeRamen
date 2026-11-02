using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Menu.Services;
using MyHomeRamen.Features.Orders.ExternalApi;
using MyHomeRamen.Features.Orders.Services;
using MyHomeRamen.Features.Payments.ExternalApi;
using MyHomeRamen.Features.Payments.Services;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Restaurants.Services;
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
            services.AddScoped<IOrdersService, OrdersService>();

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
            services.AddScoped<IIdentityService, IdentityService>();
            
            return services;
        }

        internal IServiceCollection AddRestaurantsModule(DatabaseConfigurationProvider configurationProvider)
        {
            services.AddRestaurantsPersistance(configurationProvider);
            services.AddScoped<IRestaurantService, RestaurantService>();

            return services;
        }
    }
}
