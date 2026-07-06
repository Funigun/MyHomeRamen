using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Features.Reservations.Features.Abstractions;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Orders;
using MyHomeRamen.Persistance.Payments;
using MyHomeRamen.Persistance.Reservations;
using MyHomeRamen.Persistance.ShoppingCart;
using MyHomeRamen.Persistance.Users;
using MyHomeRamen.Features.Orders.Features.Abstractions;

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
        services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<Features.Menu.Features.Products.Common.IProductRepository>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<Features.Menu.Features.Ingredients.Common.IIngredientRepository>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<Features.Menu.Features.Users.Common.IUserRepository>(provider => provider.GetRequiredService<MenuDbContext>());

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
        services.AddScoped<IBasketRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<IBasketItemRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<Features.ShoppingCart.Features.Products.Common.IProductRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<Features.ShoppingCart.Features.Ingredients.Common.IIngredientRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<Features.ShoppingCart.Features.Users.Common.IUserRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<Features.ShoppingCart.Features.Roles.Common.IRoleRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<Features.ShoppingCart.Features.Permissions.Common.IPermissionRepository>(provider => provider.GetRequiredService<ShoppingCartDbContext>());

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
        services.AddScoped<IBookingRepository>(provider => provider.GetRequiredService<ReservationsDbContext>());
        services.AddScoped<ITableRepository>(provider => provider.GetRequiredService<ReservationsDbContext>());
        services.AddScoped<Features.Reservations.Features.Users.Common.IUserRepository>(provider => provider.GetRequiredService<ReservationsDbContext>());
        services.AddScoped<Features.Reservations.Features.Roles.Common.IRoleRepository>(provider => provider.GetRequiredService<ReservationsDbContext>());
        services.AddScoped<Features.Reservations.Features.Permissions.Common.IPermissionRepository>(provider => provider.GetRequiredService<ReservationsDbContext>());

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
        services.AddScoped<IPaymentMethodRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<IPaymentChannelRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<IPaymentGatewayRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<Features.Payments.Features.Orders.Common.IOrderRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<Features.Payments.Features.Users.Common.IUserRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<Features.Payments.Features.Roles.Common.IRoleRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<Features.Payments.Features.Permissions.Common.IPermissionRepository>(provider => provider.GetRequiredService<PaymentsDbContext>());

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
