using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Identity;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(IdentityWebApiFactory))]

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

public sealed class IdentityWebApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1434)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("MyHomeRamenIdentityTestDb")
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
                                                                      .Build();

    public IdentityDbContext UsersDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        IdentityFakeUser user = new();
        DbContextOptions<IdentityDbContext> options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(_sqlContainer.GetConnectionString())
            .Options;

        UsersDbContext = new IdentityDbContext(options, IdentityFakeRestaurantConfig.Create(), user);
        await UsersDbContext.Database.MigrateAsync();
        await DataSeeder.SeedIdentityModule(UsersDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
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
            services.ReconfigureIdentityDatabase(_sqlContainer.GetConnectionString());
            services.ReconfigureIdentityTokenOptions();
            services.ReconfigureCache();
            services.ReplaceWithNoop<IKeycloakAdminService>();
        })
        .UseEnvironment("Test");
    }
}
