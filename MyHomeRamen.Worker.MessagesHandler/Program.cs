using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.Persistance;
using MyHomeRamen.ServiceDefaults;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler;
using MyHomeRamen.Worker.MessagesHandler.Common;
using Serilog;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Features;

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

    builder.AddWorkerServiceDefaults();

    // Add required database persistence
    builder.Services.AddSharedServices();
    builder.Services.AddIdentityPersistance(databaseConfigurationProvider);
    builder.Services.AddMenuPersistance(databaseConfigurationProvider);
    builder.Services.AddBasketPersistance(databaseConfigurationProvider);
    builder.Services.AddOrdersPersistance(databaseConfigurationProvider);
    builder.Services.AddReservationsPersistance(databaseConfigurationProvider);
    builder.Services.AddPaymentsPersistance(databaseConfigurationProvider);
    builder.Services.AddCacheService();

    // RabbitMq configuration
    builder.AddRabbitMQClient(ServiceNames.RabbitMq(configurationProvider.InfrastructurePrefix));
    builder.Services.AddMessagingService();

    // Register handlers
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
