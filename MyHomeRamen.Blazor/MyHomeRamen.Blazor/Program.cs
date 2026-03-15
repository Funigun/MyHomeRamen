using MudBlazor.Services;
using MyHomeRamen.Blazor.Common.Configuration;
using MyHomeRamen.Blazor.Components;
using MyHomeRamen.Blazor.Presentation;
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
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

    builder.Services.AddScoped<RestaurantConfiguration>();
    builder.Services.AddScoped<ThemeProviderService>();
    string infrastructurePrefix = builder.Configuration["RestaurantConfiguration:InfrastructurePrefix"]!;

    builder.AddBlazorServiceDefaults($"{infrastructurePrefix}-blazor");

    builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents();

    builder.Services.AddHttpContextAccessor()
                    .AddAuthenticationHandlers()
                    .AddKeycloackAuthentication(builder)
                    .AddCascadingAuthenticationState();

    builder.Services.AddMudServices();

    builder.Services.AddApiServices(infrastructurePrefix);

    WebApplication app = builder.Build();

    app.MapDefaultEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();

    app.MapRazorComponents<App>()
       .AddInteractiveServerRenderMode()
       .AddInteractiveWebAssemblyRenderMode()
       .AddAdditionalAssemblies(typeof(MyHomeRamen.Blazor.Client._Imports).Assembly);

    app.MapAutheticationEndpoints();

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
