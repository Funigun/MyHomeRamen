using System.Reflection;
using FluentValidation;
using MyHomeRamen.Api.Common;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Api.Menu;
using MyHomeRamen.Api.Orders;
using MyHomeRamen.Api.Payments;
using MyHomeRamen.Api.Reservations;
using MyHomeRamen.Api.ShoppingCart;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly apiAssembly = Assembly.GetExecutingAssembly();

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
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);
    DatabaseConfigurationProvider databaseConfigurationProvider = new(builder.Configuration);

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
                    .AddEndpointHandlers(apiAssembly)
                    .AddAuthorizationPolicies(apiAssembly)
                    .AddValidatorsFromAssembly(apiAssembly);

    builder.Services.AddMenuModule(databaseConfigurationProvider);
    builder.Services.AddShoppingCartModule(databaseConfigurationProvider);
    builder.Services.AddOrdersModule(databaseConfigurationProvider);
    builder.Services.AddReservationsModule(databaseConfigurationProvider);
    builder.Services.AddPaymentsModule(databaseConfigurationProvider);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .ConfigureAuthorizationPolicies();

    if (!isTestingEnvironment)
    {
        builder.AddRedisClient(ServiceNames.Cache(configurationProvider.InfrastructurePrefix));
        IConnectionMultiplexer? redis = builder.Services.BuildServiceProvider().GetService<IConnectionMultiplexer>();

        builder.AddRabbitMQClient(ServiceNames.RabbitMq(configurationProvider.InfrastructurePrefix));

        builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis))
                        .AddCacheService()
                        .AddMessagingService();
    }

    WebApplication app = builder.Build();

    app.UseMiddlewares();
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
