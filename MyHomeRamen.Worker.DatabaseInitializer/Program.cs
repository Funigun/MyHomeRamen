using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Infrastructure.Cache;
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
    builder.Services.AddScoped<DatabaseConfigurationProvider>();

    DatabaseConfigurationProvider databaseConfigurationProvider = new(builder.Configuration);

    builder.AddWorkerServiceDefaults();

    builder.Services.AddQuartzServices(q =>
    {
        JobKey jobKey = new(nameof(DbInitializerJob));

        q.AddJob<DbInitializerJob>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{nameof(DbInitializerJob)}-trigger")
            .StartNow()
        );
    });

    builder.Services.AddScoped<ICurrentUser, WorkerUser>();

    builder.Services.AddIdentityPersistance(databaseConfigurationProvider);
    builder.Services.AddMenuPersistance(databaseConfigurationProvider);
    builder.Services.AddBasketPersistance(databaseConfigurationProvider);
    builder.Services.AddOrdersPersistance(databaseConfigurationProvider);
    builder.Services.AddReservationsPersistance(databaseConfigurationProvider);
    builder.Services.AddPaymentsPersistance(databaseConfigurationProvider);
    builder.Services.AddCacheService();

    builder.Services.AddQuartzHostedService(options =>
    {
        options.WaitForJobsToComplete = true;
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
