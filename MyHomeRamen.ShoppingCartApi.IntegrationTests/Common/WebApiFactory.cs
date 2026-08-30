using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Payments.ExternalApi;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.Persistance.ShoppingCart;
using MyHomeRamen.Persistance.Identity;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;
using NSubstitute;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IShoppingCartDbContext ShoppingCartDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public  MyHomeRamen.IntegrationTests.Identity.IdentityTestData IdentityTestData { get; init; } = new();

    private readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};", StringComparison.OrdinalIgnoreCase);

    private ServiceProvider? _seedServiceProvider;
    private IServiceScope? _seedScope;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<ShoppingCartDbContext> options = new DbContextOptionsBuilder<ShoppingCartDbContext>().UseSqlServer(_connectionString).Options;

        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddSingleton<ICurrentUser>(user);
        services.AddScoped<ShoppingCartDbContext>(provider => new ShoppingCartDbContext(options, user, provider));
        services.AddScoped<IShoppingCartDbContext>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasktetItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddCacheService();

        _seedServiceProvider = services.BuildServiceProvider();
        _seedScope = _seedServiceProvider.CreateScope();

        ShoppingCartDbContext = _seedScope.ServiceProvider.GetRequiredService<IShoppingCartDbContext>();
        await ShoppingCartDbContext.Migrate(TestContext.Current.CancellationToken);

        await IdentityTestData.SetIdentityService(_seedScope.ServiceProvider.GetRequiredService<ICurrentUser>(), _connectionString);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        if (ShoppingCartDbContext is IAsyncDisposable asyncDisposable)
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
            services.AddScoped<IShoppingCartDbContext>(provider => provider.GetRequiredService<ShoppingCartDbContext>());
            services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
            services.ReconfigureDbContext<ShoppingCartDbContext>(_connectionString);
            services.ReconfigureDbContext<IdentityDbContext>(_connectionString);
            services.ReconfigureCache(redisFixture.ConnectionString);
            services.ReconfigureTokenOptions();
            services.ReconfigureClaimsTransformation();

            MockMenuService(services);
            MockPaymentsService(services);

        })
        .UseEnvironment("Test");
    }

    private static void MockMenuService(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMenuService));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        IMenuService menuService = Substitute.For<IMenuService>();

        menuService.ValidateProductConfigurationAsync(
                Arg.Is<Guid>(id => ShoppingCartDataSet.OriginalProductIds.Contains(id)),
                Arg.Any<List<Guid>>(),
                Arg.Any<List<Guid>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        menuService.GetProductWithSelectedIngredientsAsync(
                Arg.Is<Guid>(id => ShoppingCartDataSet.OriginalProductIds.Contains(id)),
                Arg.Any<List<Guid>>(),
                Arg.Any<List<Guid>>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                Guid productId = callInfo.ArgAt<Guid>(0);
                List<Guid> baseIngredientIds = callInfo.ArgAt<List<Guid>>(1);
                List<Guid> customIngredientIds = callInfo.ArgAt<List<Guid>>(2);

                IReadOnlyList<MenuIngredientResult> baseIngredients = baseIngredientIds
                    .Select((id, index) => new MenuIngredientResult(id, $"Base Ingredient {index}", "Base ingredient", 1m))
                    .ToList();

                IReadOnlyList<MenuIngredientResult> customIngredients = customIngredientIds
                    .Select((id, index) => new MenuIngredientResult(id, $"Custom Ingredient {index}", "Custom ingredient", 1m))
                    .ToList();

                return Task.FromResult<MenuProductResult?>(new MenuProductResult(
                    productId,
                    "Test Product",
                    "Test Product Description",
                    10m,
                    string.Empty,
                    baseIngredients,
                    customIngredients));
            });

        services.AddSingleton(menuService);
    }

    private static void MockPaymentsService(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPaymentService));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        IPaymentService paymentsService = Substitute.For<IPaymentService>();
        paymentsService.ValidatePaymentSelectionAsync(Arg.Is<Guid>(id => ShoppingCartDataSet.PaymentMethods.Keys.Contains(id)), Arg.Is<Guid>(id => ShoppingCartDataSet.PaymentMethods.Values.Contains(id)), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult(true));

        services.AddSingleton(paymentsService);
    }
}
