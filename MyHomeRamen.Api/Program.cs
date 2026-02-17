using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Persistance;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom
             .Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build())
             .CreateLogger();

try
{
    builder.AddApiServiceDefaults("my-home-ramen-api");
    builder.Services.AddScoped<RestaurantConfigurationProvider>();
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);

    builder.Services.AddIdentityPersistance(configurationProvider);
    builder.Services.AddMenuPersistance(configurationProvider);
    builder.Services.AddBasketPersistance(configurationProvider);
    builder.Services.AddOrdersPersistance(configurationProvider);
    builder.Services.AddReservationsPersistance(configurationProvider);
    builder.Services.AddPaymentsPersistance(configurationProvider);

    builder.Services.AddProblemDetails();

    builder.Services.AddOpenApi();

    WebApplication app = builder.Build();

    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapDefaultEndpoints();

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
