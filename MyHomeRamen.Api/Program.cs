using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Authorization;
using MyHomeRamen.Api.DependencyInjection;
using MyHomeRamen.Api.Middlewares;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Features;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

        builder.AddApiServiceDefaults();
    }

    builder.Services.AddSerilog();

    builder.Services.AddOpenApi("v1", options =>
                                {
                                    options.AddDocumentTransformer<TokenTransformer>();
                                    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
                                })
                    .AddProblemDetails();

    builder.Services.AddSharedServices()
                    .AddPermissionCatalogServices()
                    .AddEndpoints(featuresAssembly)
                    .AddCommandHandlers(featuresAssembly)
                    .AddQueryHandlers(featuresAssembly)
                    .AddAuthorizationPolicies(featuresAssembly);
    builder.Services.AddValidatorsFromAssemblies([featuresAssembly]);

    builder.Services.AddMenuModule(databaseConfigurationProvider)
                    .AddShoppingCartModule(databaseConfigurationProvider)
                    .AddOrdersModule(databaseConfigurationProvider)
                    .AddReservationsModule(databaseConfigurationProvider)
                    .AddPaymentsModule(databaseConfigurationProvider)
                    .AddUsersModule(databaseConfigurationProvider, builder.Configuration)
                    .AddRestaurantsModule(databaseConfigurationProvider);

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
    app.UseMiddleware<UserLoginMiddleware>();
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
