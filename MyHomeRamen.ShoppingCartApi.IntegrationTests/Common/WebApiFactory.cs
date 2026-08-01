using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Common.Contracts.Payments;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.Persistance.ShoppingCart;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;
using NSubstitute;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common;

public sealed class WebApiFactory(DbContainerFixture dbFixture, RedisFixture redisFixture) : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    public IShoppingCartDbContext ShoppingCartDbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    private readonly string _connectionString = dbFixture.ConnectionString.Replace("Database=master;", $"Database = testdb_{Guid.NewGuid()};");

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        FakeUser user = new();
        DbContextOptions<ShoppingCartDbContext> options = new DbContextOptionsBuilder<ShoppingCartDbContext>().UseSqlServer(_connectionString).Options;
        ShoppingCartDbContext = new ShoppingCartDbContext(options, user);

        await DataSeeder.SeedDatabase(ShoppingCartDbContext);

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await ((IAsyncDisposable)ShoppingCartDbContext).DisposeAsync();

        HttpClient.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReconfigureDbContext<ShoppingCartDbContext>(_connectionString);
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
                    .Select((Guid id, int index) => new MenuIngredientResult(id, $"Base Ingredient {index}", "Base ingredient", 1m))
                    .ToList();

                IReadOnlyList<MenuIngredientResult> customIngredients = customIngredientIds
                    .Select((Guid id, int index ) => new MenuIngredientResult(id, $"Custom Ingredient {index}", "Custom ingredient", 1m))
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
