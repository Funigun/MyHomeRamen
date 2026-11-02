using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Identity;
using MyHomeRamen.Persistance.Restaurants;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IMenuDbContext MenuDbContext { get; private set; } = default!;

    public Guid RestaurantId { get; private set; }

    public HttpClient HttpClient { get; private set; } = default!;

    private MyHomeRamen.IntegrationTests.Identity.IdentityTestData SharedIdentityTestData { get; } = new();

    public IdentityTestData IdentityTestData { get; private set; } = default!;

    internal readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Random.Shared.Next(1, 10000)};", StringComparison.OrdinalIgnoreCase);

    private FakeUser _seedUser = default!;
    private DbContextOptions<MenuDbContext> _menuDbContextOptions = default!;
    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_connectionString).Options;
        DbContextOptions<RestaurantsDbContext> restaurantsOptions = new DbContextOptionsBuilder<RestaurantsDbContext>().UseSqlServer(_connectionString).Options;
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

        RestaurantsDbContext restaurantsDbContext = new(restaurantsOptions, user);
        await restaurantsDbContext.Migrate(TestContext.Current.CancellationToken);
        Company company = Company.Create(
            new CompanyId(TestTenant.CompanyId),
            "My Home Ramen Integration",
            null,
            null,
            "My Home Ramen Integration Ltd",
            "PL123456789");
        Restaurant restaurant = Restaurant.Create(
            "Integration Restaurant",
            Address.Create("Main Street 1", "Warsaw", "00-001", Location.Create(52.2297, 21.0122)),
            true);
        restaurant.UpdateContactDetails("+48123456789", "integration@example.test");
        restaurant.UpdateBankAccount("1234567890", "Integration Bank", "123456789");
        company.AddRestaurant(restaurant);
        restaurantsDbContext.Companies.Add(company);
        await restaurantsDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        RestaurantId = restaurant.Id.Value;
        companyContext.SetRestaurant(RestaurantId, TestTenant.CompanyId);
        await restaurantsDbContext.DisposeAsync();

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
            services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
            services.ReconfigureDbContext<MenuDbContext>(_connectionString);
            services.ReconfigureDbContext<IdentityDbContext>(_connectionString);
            services.ReconfigureDbContext<RestaurantsDbContext>(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
