using System.Reflection;
using FluentValidation;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Api.Common.Middleware;
using MyHomeRamen.Api.Menu;
using MyHomeRamen.Api.Orders;
using MyHomeRamen.Api.Payments;
using MyHomeRamen.Api.Reservations;
using MyHomeRamen.Api.ShoppingCart;
using MyHomeRamen.Api.Users;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Features;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly apiAssembly = Assembly.GetExecutingAssembly();
Assembly featuresAssembly = typeof(MyHomeRamen.Features.DependencyInjection).Assembly;

Log.Logger = new LoggerConfiguration().ReadFrom
             .Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build())
             .CreateLogger();

try
{
    builder.Services.AddScoped<RestaurantConfigurationProvider>();
    builder.Services.AddScoped<DatabaseConfigurationProvider>();
    builder.Services.AddScoped<AuthorizationConfiguration>();
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);
    DatabaseConfigurationProvider databaseConfigurationProvider = new(builder.Configuration);
    AuthorizationConfiguration authorizationConfiguration = new(builder.Configuration);

    builder.AddConfiguration();
    bool isTestingEnvironment = builder.IsTesting();

    if (!isTestingEnvironment)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(ServiceNames.Blazor(configurationProvider.InfrastructurePrefix))
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.AddApiServiceDefaults(ServiceNames.Api(configurationProvider.InfrastructurePrefix));
    }

    builder.Services.AddSerilog();

    builder.Services.AddOpenApi("v1", options =>
                                {
                                    options.AddDocumentTransformer<TokenTransformer>();
                                    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
                                })
                    .AddProblemDetails();

    builder.Services.AddSharedServices()
                    .AddEndpoints(apiAssembly)
                    .AddEndpoints(featuresAssembly)
                    .AddCommandHandlers(apiAssembly)
                    .AddCommandHandlers(featuresAssembly)
                    .AddQueryHandlers(apiAssembly)
                    .AddQueryHandlers(featuresAssembly)
                    .AddAuthorizationPolicies(apiAssembly)
                    .AddAuthorizationPolicies(featuresAssembly);
    builder.Services.AddValidatorsFromAssemblies([apiAssembly, featuresAssembly]);

    builder.Services.AddMenuModule(databaseConfigurationProvider)
                    .AddShoppingCartModule(databaseConfigurationProvider)
                    .AddOrdersModule(databaseConfigurationProvider)
                    .AddReservationsModule(databaseConfigurationProvider)
                    .AddPaymentsModule(databaseConfigurationProvider)
                    .AddUsersModule(databaseConfigurationProvider, builder.Configuration);

    builder.Services.ConfigureAuthentication(authorizationConfiguration)
                    .ConfigureAuthorizationPolicies();

    if (!isTestingEnvironment)
    {
        builder.AddRedisClient(ServiceNames.Cache(configurationProvider.InfrastructurePrefix));

        builder.AddRabbitMQClient(ServiceNames.RabbitMq(configurationProvider.InfrastructurePrefix));

        builder.AddRedisDistributedCache(ServiceNames.Cache(configurationProvider.InfrastructurePrefix));
        builder.Services.AddCacheService()
                        .AddMessagingService();
    }

    WebApplication app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<LoggingMiddleware>();
    app.UseMiddleware<PerformanceMiddleware>();

    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options.ConfigureScalarOptions(configurationProvider));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseSerilogRequestLogging();
    app.MapDefaultEndpoints();
    app.MapEndpoints();

    if (!isTestingEnvironment)
    {
        app.UseCors();
    }

    app.UseAuthorization();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
