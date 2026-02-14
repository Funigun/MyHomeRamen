using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Orders;
using MyHomeRamen.Persistance.Payments;
using MyHomeRamen.Persistance.Reservations;
using MyHomeRamen.Persistance.ShoppingCart;
using MyHomeRamen.Persistance.Users;

namespace MyHomeRamen.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
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

        return services;
    }

    public static IServiceCollection AddBasketPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
    {
        services.AddDbContext<BasketDbContext>(options =>
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

        return services;
    }

    public static IServiceCollection AddOrdersPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
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

        return services;
    }

    public static IServiceCollection AddReservationsPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
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

        return services;
    }

    public static IServiceCollection AddPaymentsPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
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

        return services;
    }

    public static IServiceCollection AddIdentityPersistance(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
    {
        services.ConfigureIdentity();

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
        return services;
    }

    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddApiEndpoints();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 10;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 3;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
}
