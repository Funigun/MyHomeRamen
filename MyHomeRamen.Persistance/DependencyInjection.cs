using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Orders.Database;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Reservations.Database;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Orders;
using MyHomeRamen.Persistance.Payments;
using MyHomeRamen.Persistance.Reservations;
using MyHomeRamen.Persistance.ShoppingCart;
using MyHomeRamen.Persistance.Users;

namespace MyHomeRamen.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<MenuDbContext>(options =>
        {
            string? connectionString = configurationProvider.MenuConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "menu");
                }
            );
        });

        services.AddScoped<IMenuDbContext, MenuDbContext>();

        return services;
    }

    public static IServiceCollection AddBasketPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<ShoppingCartDbContext>(options =>
        {
            string? connectionString = configurationProvider.ShoppingCartConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "basket");
                }
            );
        });

        services.AddScoped<IShoppingCartDbContext, ShoppingCartDbContext>();

        return services;
    }

    public static IServiceCollection AddOrdersPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<OrdersDbContext>(options =>
        {
            string? connectionString = configurationProvider.OrdersConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders");
                }
            );
        });

        services.AddScoped<IOrdersDbContext, OrdersDbContext>();

        return services;
    }

    public static IServiceCollection AddReservationsPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<ReservationsDbContext>(options =>
        {
            string? connectionString = configurationProvider.ReservationsConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "reservations");
                }
            );
        });

        services.AddScoped<IReservationsDbContext, ReservationsDbContext>();

        return services;
    }

    public static IServiceCollection AddPaymentsPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
        {
            string? connectionString = configurationProvider.PaymentsConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "payments");
                }
            );
        });

        services.AddScoped<IPaymentsDbContext, PaymentsDbContext>();

        return services;
    }

    public static IServiceCollection AddIdentityPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            string? connectionString = configurationProvider.IdentityConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.CommandTimeout(600);
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identity");
                }
            );
        });

        services.AddScoped<IUsersDbContext, UsersDbContext>();

        return services;
    }
}
