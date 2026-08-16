using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IMenuDbContext MenuDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    internal readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Random.Shared.Next(1, 10000)};", StringComparison.OrdinalIgnoreCase);

    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_connectionString).Options;

        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddSingleton<ICurrentUser>(user);
        services.AddScoped(provider => new MenuDbContext(options, user, provider));
        services.AddScoped<IMenuDbContext>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddCacheService();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();
        MenuDbContext = _seedScope.ServiceProvider.GetRequiredService<IMenuDbContext>();
        await MenuDbContext.Migrate(TestContext.Current.CancellationToken);

        HttpClient = CreateClient();
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
            services.ReconfigureDbContext<MenuDbContext>(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
