using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.Persistance;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler;
using MyHomeRamen.Worker.MessagesHandler.Menu;
using MyHomeRamen.Worker.MessagesHandler.Orders;
using MyHomeRamen.Worker.MessagesHandler.Payments;
using MyHomeRamen.Worker.MessagesHandler.Reservations;
using MyHomeRamen.Worker.MessagesHandler.ShoppingCart;
using MyHomeRamen.Api.Common.Authorization;
using Serilog;
using System.Security.Claims;

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
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);

    builder.AddWorkerServiceDefaults($"{configurationProvider.InfrastructurePrefix}-messages-worker");

    // Add current user mock for DB contexts that require AuditableEntity updates
    builder.Services.AddScoped<ICurrentUser, DummyWorkerCurrentUser>();

    // Add required database persistence
    builder.Services.AddIdentityPersistance(configurationProvider);
    builder.Services.AddMenuPersistance(configurationProvider);
    builder.Services.AddBasketPersistance(configurationProvider);
    builder.Services.AddOrdersPersistance(configurationProvider);
    builder.Services.AddReservationsPersistance(configurationProvider);
    builder.Services.AddPaymentsPersistance(configurationProvider);

    // RabbitMq configuration
    builder.AddRabbitMQClient($"{configurationProvider.InfrastructurePrefix}-rabbitmq");
    builder.Services.AddMessagingService();

    // Register handlers
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, ShoppingCartUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, OrdersUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, MenuUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, PaymentsUserRegisteredHandler>();
    builder.Services.AddScoped<IIntegrationEventHandler<UserRegisteredIntegrationEvent>, ReservationsUserRegisteredHandler>();

    builder.Services.AddHostedService<Worker>();

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

public class DummyWorkerCurrentUser : ICurrentUser
{
    public string Id { get; init; } = "Messages Worker";
    public Guid RestaurantId { get; init; } = Guid.Empty;
    public IEnumerable<Claim> Claims { get; init; } = [];
}
