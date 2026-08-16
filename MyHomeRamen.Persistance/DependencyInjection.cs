using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
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
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Persistance.Identity;
using MyHomeRamen.Features.Identity.Abstractions;
using MenuModule = MyHomeRamen.Features.Menu.Features;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

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
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        services.AddScoped<IMenuDbContext>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<MenuModule.Products.Common.IProductRepository, Menu.ProductRepository>();
        services.AddScoped<MenuModule.Ingredients.Common.IIngredientRepository, Menu.IngredientRepository>();

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
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        services.AddScoped<IShoppingCartDbContext, ShoppingCartDbContext>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasktetItemRepository>();
        services.AddScoped<IProductRepository, ShoppingCart.ProductRepository>();
        services.AddScoped<Features.ShoppingCart.Features.Ingredients.Common.IIngredientRepository, ShoppingCart.IngredientRepository>();
        services.AddScoped<Features.ShoppingCart.Features.Users.Common.IUserRepository, ShoppingCart.UserRepository>();
        services.AddScoped<Features.ShoppingCart.Features.Roles.Common.IRoleRepository, ShoppingCart.RoleRepository>();
        services.AddScoped<Features.ShoppingCart.Features.Permissions.Common.IPermissionRepository, ShoppingCart.PermissionRepository>();

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
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
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
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        services.AddScoped<IReservationsDbContext, ReservationsDbContext>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<Features.Reservations.Features.Users.Common.IUserRepository, Reservations.UserRepository>();
        services.AddScoped<Features.Reservations.Features.Roles.Common.IRoleRepository, Reservations.RoleRepository>();
        services.AddScoped<Features.Reservations.Features.Permissions.Common.IPermissionRepository, Reservations.PermissionRepository>();

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
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        services.AddScoped<IPaymentsDbContext, PaymentsDbContext>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IPaymentChannelRepository, PaymentChannelRepository>();
        services.AddScoped<IPaymentGatewayRepository, PaymentGatewayRepository>();
        services.AddScoped<Features.Payments.Features.Orders.Common.IOrderRepository, Payments.OrderRepository>();

        return services;
    }

    public static IServiceCollection AddIdentityPersistance(this IServiceCollection services, DatabaseConfigurationProvider configurationProvider)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            string? connectionString = configurationProvider.IdentityConnectionString;
            options.UseSqlServer(
                connectionString,
                serverOptions =>
                {
                    serverOptions.CommandTimeout(600);
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identity");
                    serverOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        services.AddScoped<IIdentityDbContext, IdentityDbContext>();
        services.AddScoped<Features.Identity.Features.Users.Common.IUserRepository, Identity.UserRepository>();
        services.AddScoped<Features.Identity.Features.Roles.Common.IRoleRepository, Identity.RoleRepository>();

        return services;
    }
}
