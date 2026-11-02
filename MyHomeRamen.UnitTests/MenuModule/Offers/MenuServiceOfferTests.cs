using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Offers.Common;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;
using MyHomeRamen.Features.Menu.Services;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using NSubstitute;

namespace MyHomeRamen.UnitTests.MenuModule.Offers;

public sealed class MenuServiceOfferTests
{
    [Fact]
    public async Task GetOfferAsync_ShouldReturnOnlyPricedCustomIngredients_WhenOfferIsAvailable()
    {
        Guid companyId = Guid.CreateVersion7();
        Guid restaurantId = Guid.CreateVersion7();
        Ingredient pricedExtra = CreateIngredient(companyId, "Priced extra");
        Ingredient unpricedExtra = CreateIngredient(companyId, "Unpriced extra");
        Product product = CreateProduct(companyId, [pricedExtra, unpricedExtra]);
        RestaurantProductOffer offer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            restaurantId,
            companyId,
            product,
            15m);
        IMenuDbContext dbContext = Substitute.For<IMenuDbContext>();
        IRestaurantProductOfferRepository offerRepository = Substitute.For<IRestaurantProductOfferRepository>();
        IRestaurantProductOfferQuery offerQuery = Substitute.For<IRestaurantProductOfferQuery>();
        IRestaurantIngredientPriceRepository priceRepository = Substitute.For<IRestaurantIngredientPriceRepository>();
        IRestaurantIngredientPriceQuery priceQuery = Substitute.For<IRestaurantIngredientPriceQuery>();
        CompanyContext companyContext = CreateCompanyContext(restaurantId, companyId);
        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();

        dbContext.RestaurantProductOffer.Returns(offerRepository);
        offerRepository.Query().Returns(offerQuery);
        offerQuery.ById(offer.Id, Arg.Any<CancellationToken>()).Returns(offer);
        dbContext.RestaurantIngredientPrice.Returns(priceRepository);
        priceRepository.Query().Returns(priceQuery);
        priceQuery.ByIngredientIds(Arg.Any<IEnumerable<IngredientId>>(), Arg.Any<CancellationToken>())
            .Returns([new RestaurantIngredientPriceDto(pricedExtra.Id.Value, 2.50m)]);
        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, companyId, true));

        MenuService menuService = new(dbContext, companyContext, restaurantService);

        MenuOfferResult? result = await menuService.GetOfferAsync(restaurantId, offer.Id.Value, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(15m, result.Price);
        Assert.Collection(
            result.AvailableExtras,
            extra =>
            {
                Assert.Equal(pricedExtra.Id.Value, extra.Id);
                Assert.Equal(2.50m, extra.Price);
            });
    }

    [Fact]
    public async Task ValidateProductConfigurationAsync_ShouldRejectUnpricedCustomIngredient()
    {
        Guid companyId = Guid.CreateVersion7();
        Guid restaurantId = Guid.CreateVersion7();
        Ingredient customIngredient = CreateIngredient(companyId, "Custom extra");
        Product product = CreateProduct(companyId, [customIngredient]);
        RestaurantProductOffer offer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            restaurantId,
            companyId,
            product,
            15m);
        IMenuDbContext dbContext = Substitute.For<IMenuDbContext>();
        IRestaurantProductOfferRepository offerRepository = Substitute.For<IRestaurantProductOfferRepository>();
        IRestaurantProductOfferQuery offerQuery = Substitute.For<IRestaurantProductOfferQuery>();
        IRestaurantIngredientPriceRepository priceRepository = Substitute.For<IRestaurantIngredientPriceRepository>();
        IRestaurantIngredientPriceQuery priceQuery = Substitute.For<IRestaurantIngredientPriceQuery>();
        CompanyContext companyContext = CreateCompanyContext(restaurantId, companyId);
        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();

        dbContext.RestaurantProductOffer.Returns(offerRepository);
        offerRepository.Query().Returns(offerQuery);
        offerQuery.ByProductId(product.Id, Arg.Any<CancellationToken>()).Returns(offer);
        dbContext.RestaurantIngredientPrice.Returns(priceRepository);
        priceRepository.Query().Returns(priceQuery);
        priceQuery.ByIngredientIds(Arg.Any<IEnumerable<IngredientId>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<RestaurantIngredientPriceDto>());
        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, companyId, true));

        MenuService menuService = new(dbContext, companyContext, restaurantService);

        bool isValid = await menuService.ValidateProductConfigurationAsync(
            product.Id.Value,
            [],
            [customIngredient.Id.Value],
            CancellationToken.None);

        Assert.False(isValid);
    }

    [Fact]
    public async Task ValidateProductConfigurationAsync_ShouldRejectCrossCompanyRestaurant()
    {
        Guid companyId = Guid.CreateVersion7();
        Guid restaurantId = Guid.CreateVersion7();
        Guid otherCompanyId = Guid.CreateVersion7();
        IMenuDbContext dbContext = Substitute.For<IMenuDbContext>();
        CompanyContext companyContext = CreateCompanyContext(restaurantId, companyId);
        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();

        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, otherCompanyId, true));

        MenuService menuService = new(dbContext, companyContext, restaurantService);

        bool isValid = await menuService.ValidateProductConfigurationAsync(
            Guid.CreateVersion7(),
            [],
            [],
            CancellationToken.None);

        Assert.False(isValid);
    }

    private static CompanyContext CreateCompanyContext(Guid restaurantId, Guid companyId)
    {
        CompanyContext companyContext = new();
        companyContext.SetRestaurant(restaurantId, companyId);
        return companyContext;
    }

    private static Ingredient CreateIngredient(Guid companyId, string name)
        => Ingredient.Create(
            new IngredientId(Guid.CreateVersion7()),
            companyId,
            name,
            "A valid ingredient description.",
            []);

    private static Product CreateProduct(Guid companyId, IEnumerable<Ingredient> customIngredients)
        => Product.Create(
            new ProductId(Guid.CreateVersion7()),
            companyId,
            "Valid product name",
            "A valid product description for testing.",
            string.Empty,
            [],
            new Collection<Ingredient>(customIngredients.ToList()),
            []);
}
