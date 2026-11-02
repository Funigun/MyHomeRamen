using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Offers;

public sealed class MenuOfferIntegrationTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task GetOfferAsync_ShouldReturnPricedExtrasOnly_ForRestaurant()
    {
        OfferData data = await SeedOfferAsync();

        using IServiceScope scope = apiFactory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ICompanyContext>()
            .SetRestaurant(apiFactory.RestaurantId, TestTenant.CompanyId);
        IMenuService menuService = scope.ServiceProvider.GetRequiredService<IMenuService>();

        MenuOfferResult? result = await menuService.GetOfferAsync(
            apiFactory.RestaurantId,
            data.Offer.Id.Value,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(17.50m, result.Price);
        Assert.Collection(
            result.AvailableExtras,
            extra =>
            {
                Assert.Equal(data.PricedExtra.Id.Value, extra.Id);
                Assert.Equal(2.25m, extra.Price);
            });
    }

    [Fact]
    public async Task GetOfferAsync_ShouldHideOffer_WhenProductIsRetired()
    {
        OfferData data = await SeedOfferAsync(retireProduct: true);

        using IServiceScope scope = apiFactory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ICompanyContext>()
            .SetRestaurant(apiFactory.RestaurantId, TestTenant.CompanyId);
        IMenuService menuService = scope.ServiceProvider.GetRequiredService<IMenuService>();

        MenuOfferResult? result = await menuService.GetOfferAsync(
            apiFactory.RestaurantId,
            data.Offer.Id.Value,
            TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task RestaurantProductOffer_ShouldRejectDuplicateRestaurantProductPair()
    {
        OfferData data = await SeedOfferAsync();
        RestaurantProductOffer duplicate = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            data.Product,
            19m);

        await using MenuDbContext duplicateContext = apiFactory.CreateMenuDbContextForTenant(
            apiFactory.RestaurantId,
            TestTenant.CompanyId);
        duplicateContext.Products.Attach(data.Product);
        duplicateContext.RestaurantProductOffers.Add(duplicate);

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            duplicateContext.SaveChangesAsync(TestContext.Current.CancellationToken));
    }

    private async Task<OfferData> SeedOfferAsync(bool retireProduct = false)
    {
        Ingredient pricedExtra = CreateIngredient("Priced extra");
        Ingredient unpricedExtra = CreateIngredient("Unpriced extra");
        Product product = Product.Create(
            new ProductId(Guid.CreateVersion7()),
            TestTenant.CompanyId,
            $"Integration ramen {Guid.NewGuid():N}",
            "A valid product description for integration testing.",
            string.Empty,
            new Collection<Ingredient>(),
            new Collection<Ingredient>([pricedExtra, unpricedExtra]),
            []);
        if (retireProduct)
        {
            product.Retire();
        }

        RestaurantProductOffer offer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            product,
            17.50m);
        RestaurantIngredientPrice extraPrice = RestaurantIngredientPrice.Create(
            new RestaurantIngredientPriceId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            pricedExtra,
            2.25m);

        apiFactory.MenuDbContext.Ingredient.AddRange([pricedExtra, unpricedExtra]);
        apiFactory.MenuDbContext.Product.Add(product);
        apiFactory.MenuDbContext.RestaurantProductOffer.Add(offer);
        apiFactory.MenuDbContext.RestaurantIngredientPrice.Add(extraPrice);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return new OfferData(product, offer, pricedExtra);
    }

    private static Ingredient CreateIngredient(string name)
        => Ingredient.Create(
            new IngredientId(Guid.CreateVersion7()),
            TestTenant.CompanyId,
            name,
            "A valid extra ingredient description.",
            []);

    private sealed record OfferData(Product Product, RestaurantProductOffer Offer, Ingredient PricedExtra);
}
