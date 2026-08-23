using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.IntegrationTests.Identity;
using MyHomeRamen.Persistance.Identity;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

public sealed class IdentityWebApiFactory(DbContainerFixture dbContainerFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IIdentityDbContext IdentityDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public IdentityTestData IdentityTestData { get; private set; } = new();

    private readonly string _connectionString = dbContainerFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};", StringComparison.OrdinalIgnoreCase);

    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    public IServiceScope CreateSeedScope()
    {
        return _seedServiceProvider?.CreateScope()
               ?? throw new InvalidOperationException("Identity test database has not been initialized.");
    }

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        IdentityFakeUser user = new();
        DbContextOptions<IdentityDbContext> options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddScoped<ICurrentUser>(provider => user);
        services.AddScoped<IdentityDbContext>(provider => new IdentityDbContext(options, user, provider));
        services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddPermissionCatalogServices();
        services.AddScoped<IPermissionCatalogSynchronizer, PermissionCatalogSynchronizer>();
        services.AddCacheService();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();

        IdentityDbContext seedDbContext = _seedScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await seedDbContext.Database.MigrateAsync();
        IdentityDbContext = _seedScope.ServiceProvider.GetRequiredService<IIdentityDbContext>();

        await IdentityTestData.SeedAsync(_seedScope);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
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
        builder.UseSetting("RestaurantConfiguration:Name", "TestRestaurant");
        builder.UseSetting("RestaurantConfiguration:InfrastructurePrefix", "test");

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
            services.ReconfigureDbContext<IdentityDbContext>(_connectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureCache();
            services.ReplaceWithNoop<IKeycloakAdminService>();
        })
        .UseEnvironment("Test");
    }
}
