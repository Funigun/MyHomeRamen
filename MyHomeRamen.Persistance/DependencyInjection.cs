using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Orders;
using MyHomeRamen.Persistance.Payments;
using MyHomeRamen.Persistance.Reservations;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.Persistance;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMenuPersistance(IConfiguration configuration)
        {
            services.AddDbContext<MenuDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("MyHomeRamenConnectionString");
                options.UseNpgsql(
                    connectionString,
                    serverOptions =>
                    {
                        serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "menu");
                    }
                );
            });

            return services;
        }

        public IServiceCollection AddBasketPersistance(IConfiguration configuration)
        {
            services.AddDbContext<BasketDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("MyHomeRamenConnectionString");
                options.UseNpgsql(
                    connectionString,
                    serverOptions =>
                    {
                        serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "basket");
                    }
                );
            });

            return services;
        }

        public IServiceCollection AddOrdersPersistance(IConfiguration configuration)
        {
            services.AddDbContext<OrdersDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("MyHomeRamenConnectionString");
                options.UseNpgsql(
                    connectionString,
                    serverOptions =>
                    {
                        serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders");
                    }
                );
            });

            return services;
        }

        public IServiceCollection AddReservationsPersistance(IConfiguration configuration)
        {
            services.AddDbContext<ReservationsDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("MyHomeRamenConnectionString");
                options.UseNpgsql(
                    connectionString,
                    serverOptions =>
                    {
                        serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "reservations");
                    }
                );
            });

            return services;
        }

        public IServiceCollection AddPaymentsPersistance(IConfiguration configuration)
        {
            services.AddDbContext<PaymentsDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("MyHomeRamenConnectionString");
                options.UseNpgsql(
                    connectionString,
                    serverOptions =>
                    {
                        serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "payments");
                    }
                );
            });

            return services;
        }
    }
}
