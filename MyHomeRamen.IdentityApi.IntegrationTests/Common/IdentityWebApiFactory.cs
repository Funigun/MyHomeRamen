using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Persistance.Identity;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

public sealed class IdentityWebApiFactory(DbContainerFixture dbContainerFixture, DataSeeder dataSeeder) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IIdentityDbContext IdentityDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public DataSeeder DataSeeder { get; private set; } = dataSeeder;

    private readonly string _connectionString = dbContainerFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};", StringComparison.OrdinalIgnoreCase);

    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        IdentityFakeUser user = new(DataSeeder);
        RestaurantConfigurationProvider restaurantConfiguration = IdentityFakeRestaurantConfig.Create(DataSeeder);
        DbContextOptions<IdentityDbContext> options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddSingleton<ICurrentUser>(user);
        services.AddSingleton(restaurantConfiguration);
        services.AddScoped<IdentityDbContext>(provider => new IdentityDbContext(options, restaurantConfiguration, user, provider));
        services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddCacheService();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();

        IdentityDbContext seedDbContext = _seedScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await seedDbContext.Database.MigrateAsync();
        IdentityDbContext = _seedScope.ServiceProvider.GetRequiredService<IIdentityDbContext>();

        await DataSeeder.SeedIdentityModule(IdentityDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        if (IdentityDbContext is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        _seedScope?.Dispose();
        if (_seedServiceProvider is not null)
        {
            await _seedServiceProvider.DisposeAsync();
        }

        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("RestaurantConfiguration:RestaurantId", DataSeeder.SeededRestaurantId.ToString());
        builder.UseSetting("RestaurantConfiguration:Name", "TestRestaurant");
        builder.UseSetting("RestaurantConfiguration:InfrastructurePrefix", "test");

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
            services.ReconfigureIdentityDatabase(_connectionString);
            services.ReconfigureIdentityTokenOptions();
            services.ReconfigureCache();
            services.ReplaceWithNoop<IKeycloakAdminService>();
        })
        .UseEnvironment("Test");
    }
}
