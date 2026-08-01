using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Identity;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

public sealed class IdentityWebApiFactory(DbContainerFixture dbContainerFixture, DataSeeder dataSeeder) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IdentityDbContext UsersDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public DataSeeder DataSeeder { get; private set; } = dataSeeder;

    private readonly string _connectionString = dbContainerFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};", StringComparison.OrdinalIgnoreCase);
    

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        IdentityFakeUser user = new(DataSeeder);
        DbContextOptions<IdentityDbContext> options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        UsersDbContext = new IdentityDbContext(options, IdentityFakeRestaurantConfig.Create(DataSeeder), user);
        await UsersDbContext.Database.MigrateAsync();
        await DataSeeder.SeedIdentityModule(UsersDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await UsersDbContext.DisposeAsync();
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
            services.ReconfigureIdentityDatabase(_connectionString);
            services.ReconfigureIdentityTokenOptions();
            services.ReconfigureCache();
            services.ReplaceWithNoop<IKeycloakAdminService>();
        })
        .UseEnvironment("Test");
    }
}
