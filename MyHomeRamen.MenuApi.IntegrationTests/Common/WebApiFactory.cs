using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Roles;
using MyHomeRamen.Features.Menu.Features.Users.Common;
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

    private readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};", StringComparison.OrdinalIgnoreCase);

    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<MenuDbContext> options = new DbContextOptionsBuilder<MenuDbContext>().UseSqlServer(_connectionString).Options;

        // MenuDbContext resolves repositories via IServiceProvider - build seed container with same graph as AddMenuPersistance
        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddSingleton<ICurrentUser>(user);
        services.AddScoped<MenuDbContext>();
        services.AddScoped<IMenuDbContext>(provider => provider.GetRequiredService<MenuDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();
        MenuDbContext = _seedScope.ServiceProvider.GetRequiredService<IMenuDbContext>();

        await DataSeeder.SeedMenuModule(MenuDbContext);

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
        builder.ConfigureServices(services =>
        {
            services.ReconfigureDbContext<MenuDbContext>(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();
        })
        .UseEnvironment("Test");
    }
}
