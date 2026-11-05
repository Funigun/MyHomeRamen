using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.IntegrationTests.Identity;
using MyHomeRamen.IntegrationTests.Restaurants;
using MyHomeRamen.Persistance.Restaurants;
using MyHomeRamen.RestaurantsApi.IntegrationTests.Common.Data;
using MyHomeRamen.RestaurantsApi.IntegrationTests.Common.Fixtures;

namespace MyHomeRamen.RestaurantsApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture)
    : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private readonly string _connectionString = dbFixture.ConnectionString.Replace(
        "Database=master;",
        $"Database = testdb_{Guid.NewGuid():N};",
        StringComparison.OrdinalIgnoreCase);

    public HttpClient HttpClient { get; private set; } = default!;

    public Guid RestaurantId { get; private set; }

    public IdentityTestData IdentityTestData { get; private set; } = new();

    private RestaurantTestData RestaurantTestData { get; set; } = new();

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        await IdentityTestData.SetIdentityService(user, _connectionString);
        RestaurantId = await RestaurantTestData.InitializeAsync(
            _connectionString,
            user,
            TestTenant.CompanyId,
            TestContext.Current.CancellationToken);

        HttpClient = CreateClient();
    }

    public Task<Guid> SeedRestaurantAsync(string name, bool isActive = true)
        => RestaurantTestData.SeedRestaurantAsync(
            name,
            isActive,
            TestContext.Current.CancellationToken);

    public new async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReconfigureIdentityDbContext(_connectionString);
            services.ReconfigureDbContext<RestaurantsDbContext>(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
