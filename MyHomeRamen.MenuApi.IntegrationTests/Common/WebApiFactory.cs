using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.IntegrationTests.Restaurants;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IMenuDbContext MenuDbContext { get; private set; } = default!;

    public Guid RestaurantId { get; private set; }

    public HttpClient HttpClient { get; private set; } = default!;

    private MyHomeRamen.IntegrationTests.Identity.IdentityTestData SharedIdentityTestData { get; } = new();

    public IdentityTestData IdentityTestData { get; private set; } = default!;

    internal readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid():N};", StringComparison.OrdinalIgnoreCase);

    private FakeUser _seedUser = default!;
    private DbContextOptions<MenuDbContext> _menuDbContextOptions = default!;
    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;
    private RestaurantTestData _restaurantTestData = default!;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_connectionString).Options;
        CompanyContext companyContext = new();
        _seedUser = user;
        _menuDbContextOptions = options;

        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddSingleton<ICompanyContext>(companyContext);
        services.AddSingleton<ICurrentUser>(user);
        services.AddScoped(provider => new MenuDbContext(options, user, provider, companyContext));
        services.AddScoped<IMenuDbContext>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddCacheService();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();

        _restaurantTestData = new RestaurantTestData();
        RestaurantId = await _restaurantTestData.InitializeAsync(
            _connectionString,
            user,
            TestTenant.CompanyId,
            TestContext.Current.CancellationToken);
        companyContext.SetRestaurant(RestaurantId, TestTenant.CompanyId);

        MenuDbContext = _seedScope.ServiceProvider.GetRequiredService<IMenuDbContext>();
        await MenuDbContext.Migrate(TestContext.Current.CancellationToken);

        await SharedIdentityTestData.SetIdentityService(_seedScope.ServiceProvider.GetRequiredService<ICurrentUser>(), _connectionString);
        IdentityTestData = new IdentityTestData(SharedIdentityTestData);
        await IdentityTestData.SeedAsync();

        HttpClient = CreateClient();
        HttpClient.DefaultRequestHeaders.Add(HeaderConstants.RestaurantId, RestaurantId.ToString());
    }

    internal MenuDbContext CreateMenuDbContextForTenant(Guid restaurantId, Guid companyId)
    {
        CompanyContext companyContext = new();
        companyContext.SetRestaurant(restaurantId, companyId);
        return new MenuDbContext(
            _menuDbContextOptions,
            _seedUser,
            _seedScope!.ServiceProvider,
            companyContext);
    }

    internal Task<Guid> SeedRestaurantAsync(string name, bool isActive = true)
        => _restaurantTestData.SeedRestaurantAsync(
            name,
            isActive,
            TestContext.Current.CancellationToken);

    public new async Task DisposeAsync()
    {
        if (MenuDbContext is IAsyncDisposable asyncDisposable)
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
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IMenuDbContext>(provider => provider.GetRequiredService<MenuDbContext>());
            services.ReconfigureDbContext<MenuDbContext>(_connectionString);
            services.ReconfigureIdentityDbContext(_connectionString);
            services.ReconfigureRestaurantsDbContext(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
