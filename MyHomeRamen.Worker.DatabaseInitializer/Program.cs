using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Persistance;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.DatabaseInitializer;
using MyHomeRamen.Worker.DatabaseInitializer.Config;
using Quartz;
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
    RestaurantConfigurationProvider configurationProvider = new(builder.Configuration);

    builder.AddWorkerServiceDefaults("my-home-ramen-db-initializer-worker");

    // Configure Quartz with the DbInitializerJob scheduled to run once immediately
    builder.Services.AddQuartzServices(q =>
    {
        JobKey jobKey = new(nameof(DbInitializerJob));

        q.AddJob<DbInitializerJob>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{nameof(DbInitializerJob)}-trigger")
            .StartNow() // Run immediately when application starts
        );
    });

    builder.Services.AddScoped<ICurrentUser, Worker>();
    builder.Services.AddIdentityPersistance(configurationProvider);
    builder.Services.AddMenuPersistance(configurationProvider);
    builder.Services.AddBasketPersistance(configurationProvider);
    builder.Services.AddOrdersPersistance(configurationProvider);
    builder.Services.AddReservationsPersistance(configurationProvider);
    builder.Services.AddPaymentsPersistance(configurationProvider);

    builder.Services.AddQuartzHostedService(options =>
    {
        options.WaitForJobsToComplete = true; // Essential specifically for run-once jobs
    });

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
