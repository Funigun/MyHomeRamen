using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.Persistance;
using MyHomeRamen.ServiceDefaults;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler;
using MyHomeRamen.Worker.MessagesHandler.Common;
using MyHomeRamen.Worker.MessagesHandler.Menu;
using MyHomeRamen.Worker.MessagesHandler.Orders;
using MyHomeRamen.Worker.MessagesHandler.Payments;
using MyHomeRamen.Worker.MessagesHandler.Reservations;
using MyHomeRamen.Worker.MessagesHandler.ShoppingCart;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom
             .Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build())
             .CreateLogger();

try
{
    builder.AddConfiguration();
    builder.Services.AddScoped<RestaurantConfigurationProvider>();
    builder.Services.AddScoped<DatabaseConfigurationProvider>();
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);
    DatabaseConfigurationProvider databaseConfigurationProvider = new(builder.Configuration);

    builder.AddWorkerServiceDefaults(ServiceNames.MessagesWorker(configurationProvider.InfrastructurePrefix));

    // Add current user mock for DB contexts that require AuditableEntity updates
    builder.Services.AddScoped<ICurrentUser, WorkerUser>();

    // Add required database persistence
    builder.Services.AddIdentityPersistance(databaseConfigurationProvider);
    builder.Services.AddMenuPersistance(databaseConfigurationProvider);
    builder.Services.AddBasketPersistance(databaseConfigurationProvider);
    builder.Services.AddOrdersPersistance(databaseConfigurationProvider);
    builder.Services.AddReservationsPersistance(databaseConfigurationProvider);
    builder.Services.AddPaymentsPersistance(databaseConfigurationProvider);

    // RabbitMq configuration
    builder.AddRabbitMQClient(ServiceNames.RabbitMq(configurationProvider.InfrastructurePrefix));
    builder.Services.AddMessagingService();

    // Register handlers
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, ShoppingCartUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, OrdersUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, MenuUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, PaymentsUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, ReservationsUserRegisteredHandler>();

    builder.Services.AddScoped<IIntegrationEventHandler<GuestUserCreatedIntegrationEvent>, ShoppingCartGuestRegisteredHandler>();

    builder.Services.AddHostedService<UserRegistrationHandler>();
    builder.Services.AddHostedService<GuestRegistrationHandler>();

    IHost host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
