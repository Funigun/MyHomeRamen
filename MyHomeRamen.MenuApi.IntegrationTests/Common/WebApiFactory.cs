using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IMenuDbContext MenuDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(dbFixture.ConnectionString).Options;
        MenuDbContext = new MenuDbContext(options, user);

        await DataSeeder.SeedMenuModule(MenuDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await ((IAsyncDisposable)MenuDbContext).DisposeAsync();

        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReconfigureDbContext<MenuDbContext>(dbFixture.ConnectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
