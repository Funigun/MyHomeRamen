using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;
using MyHomeRamen.Persistance.Menu;
using Testcontainers.MsSql;
using Testcontainers.Redis;

[assembly: AssemblyFixture(typeof(WebApiFactory))]

namespace MyHomeRamen.IntegrationTests.Common;

public sealed class WebApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1433)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("MyHomeRamenTestDb")
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
                                                                      .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:8.2").Build();

    public MenuDbContext MenuDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _redisContainer.StartAsync();

        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_sqlContainer.GetConnectionString()).Options;
        MenuDbContext = new MenuDbContext(options, user);
        await DataSeeder.SeedMenuModule(MenuDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();

        await MenuDbContext.DisposeAsync();
        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReconfigureDatabase(_sqlContainer.GetConnectionString());
            services.ReconfigureCache(_redisContainer.GetConnectionString());
            services.ReconfigureTokenOptions();
        })
        .UseEnvironment("Test");
    }
}
