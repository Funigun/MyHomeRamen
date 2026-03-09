using System.Reflection;
using FluentValidation;
using MyHomeRamen.Api.Common;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Menu;
using MyHomeRamen.Api.Orders;
using MyHomeRamen.Api.Payments;
using MyHomeRamen.Api.Reservations;
using MyHomeRamen.Api.ShoppingCart;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Infrastructure.Messaging;
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
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins($"{configurationProvider.InfrastructurePrefix}-blazor")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.AddApiServiceDefaults($"{configurationProvider.InfrastructurePrefix}-api");
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

    builder.Services.AddMenuModule(configurationProvider);
    builder.Services.AddShoppingCartModule(configurationProvider);
    builder.Services.AddOrdersModule(configurationProvider);
    builder.Services.AddReservationsModule(configurationProvider);
    builder.Services.AddPaymentsModule(configurationProvider);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .ConfigureAuthorizationPolicies();

    builder.AddRedisClient($"{configurationProvider.InfrastructurePrefix}-cache");
    IConnectionMultiplexer? redis = builder.Services.BuildServiceProvider().GetService<IConnectionMultiplexer>();

    builder.AddRabbitMQClient($"{configurationProvider.InfrastructurePrefix}-rabbitmq");

    builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis))
                    .AddCacheService()
                    .AddMessagingService();

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
    app.UseCors();
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
