using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.ShoppingCart;
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
                                                                      .WithCleanUp(true)
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
                                                                      .Build();

    private readonly MsSqlContainer _sqlCartContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1434)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("MyHomeRamenTestCartDb")
                                                                      .WithCleanUp(true)
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1434))
                                                                      .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:8.2").Build();

    public MenuDbContext MenuDbContext { get; private set; } = default!;

    public ShoppingCartDbContext ShoppingCartDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        IEnumerable<Task> containers = [_sqlCartContainer.StartAsync(), _sqlContainer.StartAsync(), _redisContainer.StartAsync()];
        await Task.WhenAll(containers);

        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_sqlContainer.GetConnectionString()).Options;
        MenuDbContext = new MenuDbContext(options, user);

        DbContextOptions<ShoppingCartDbContext> shoppingCartOptions = new DbContextOptionsBuilder<ShoppingCartDbContext>().UseSqlServer(_sqlCartContainer.GetConnectionString()).Options;
        ShoppingCartDbContext = new ShoppingCartDbContext(shoppingCartOptions, user);

        IEnumerable<Task> dbContexts = [DataSeeder.SeedMenuModule(MenuDbContext), ShoppingCartDataSeeder.SeedShoppingCartModule(ShoppingCartDbContext)];
        await Task.WhenAll(dbContexts);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _sqlCartContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();

        await MenuDbContext.DisposeAsync();
        await ShoppingCartDbContext.DisposeAsync();

        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReconfigureDbContext<MenuDbContext>(_sqlContainer.GetConnectionString());
            services.ReconfigureDbContext<ShoppingCartDbContext>(_sqlCartContainer.GetConnectionString());
            services.ReconfigureCache(_redisContainer.GetConnectionString());
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
