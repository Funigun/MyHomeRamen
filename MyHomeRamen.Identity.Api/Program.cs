using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MyHomeRamen.Api.Common;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Identity.Api.Application.Services;
using MyHomeRamen.Identity.Api.Presentation;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Messaging;
using MyHomeRamen.Persistance;
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
    builder.AddConfiguration();
    builder.Services.AddScoped<RestaurantConfigurationProvider>();
    builder.Services.AddScoped<DatabaseConfigurationProvider>();
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);
    DatabaseConfigurationProvider databaseConfigurationProvider = new(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins($"{configurationProvider.InfrastructurePrefix}-blazor")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.AddApiServiceDefaults($"{configurationProvider.InfrastructurePrefix}-identity-api");
    builder.Services.AddSerilog();

    builder.Services.AddOpenApi("v1", options =>
                                 {
                                     options.AddDocumentTransformer<TokenTransformer>();
                                     options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
                                 })
                    .AddProblemDetails();

    builder.Services.AddSharedServices()
                    .AddScoped<AuthorizationService>()
                    .AddEndpoints(apiAssembly)
                    .AddEndpointHandlers(apiAssembly)
                    .AddAuthorizationPolicies(apiAssembly)
                    .AddValidatorsFromAssembly(apiAssembly);

    builder.Services.AddIdentityPersistance(databaseConfigurationProvider);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .ConfigureAuthorizationPolicies();

    builder.AddRedisClient($"{configurationProvider.InfrastructurePrefix}-cache");
    IConnectionMultiplexer? redis = builder.Services.BuildServiceProvider().GetService<IConnectionMultiplexer>();

    builder.AddRabbitMQClient($"{configurationProvider.InfrastructurePrefix}-rabbitmq");

    builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis))
                    .AddCacheService()
                    .AddMessagingService()
                    .AddKeycloakAdminService(builder.Configuration);

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
