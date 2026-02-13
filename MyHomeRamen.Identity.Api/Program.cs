using System.Reflection;
using FluentValidation;
using MyHomeRamen.Api.Common;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Identity.Api.Persistance;
using MyHomeRamen.Identity.Api.Presentation;
using Scalar.AspNetCore;
using Serilog;

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
    const string corsPolicyName = "RestaurantPolicy";

    builder.AddConfiguration();
    builder.Services.AddScoped<RestaurantConfigurationProvider>();
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsPolicyName, policy =>
        {
        policy.WithOrigins($"{configurationProvider.InfrastructurePrefix}-blazor")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddSerilog();
    builder.AddServiceDefaults($"{configurationProvider.InfrastructurePrefix}-identity-api");

    builder.Services.AddOpenApi("v1", options =>
    {
        options.AddDocumentTransformer<TokenTransformer>();
        options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
    });

    builder.Services.AddSharedServices()
                    .AddEndpoints(apiAssembly)
                    .AddAuthorizationPolicies(apiAssembly)
                    .AddValidatorsFromAssembly(apiAssembly);

    builder.Services.ConfigureIdentity()
                    .ConfigureDatabase(builder.Configuration);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .AddAuthorizationBuilder()
                    .AddPolicy(corsPolicyName, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

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
    app.UseCors(corsPolicyName);
    app.MapDefaultEndpoints();
    app.MapEndpoints();
    app.UseAuthorization();

    await app.InitDatabase();
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
