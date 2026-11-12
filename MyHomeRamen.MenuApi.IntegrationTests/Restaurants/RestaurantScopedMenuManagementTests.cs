using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Menu.Features.MenuCards.ActivateMenuCard;
using MyHomeRamen.Features.Menu.Features.MenuCards.AddMenuSectionOffers;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardOptions;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;
using MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuCardsOrder;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Restaurants;

public sealed class RestaurantScopedMenuManagementTests(WebApiFactory apiFactory) :
    IClassFixture<WebApiFactory>,
    IAsyncLifetime
{
    private (string KeycloakUserId, Guid UserId) _user;
    private Ingredient _firstIngredient = default!;
    private Ingredient _secondIngredient = default!;
    private RestaurantProductOffer _activeOffer = default!;
    private RestaurantProductOffer _batchOfferOne = default!;
    private RestaurantProductOffer _batchOfferTwo = default!;
    private RestaurantProductOffer _batchOfferThree = default!;
    private RestaurantProductOffer _retiredOffer = default!;
    private RestaurantProductOffer _otherRestaurantOffer = default!;
    private MenuCard _activeCard = default!;
    private MenuCard _inactiveCard = default!;
    private MenuCard _otherRestaurantCard = default!;
    private Guid _otherRestaurantId;
    private string _testSuffix = string.Empty;

    public async ValueTask InitializeAsync()
    {
        _testSuffix = Guid.CreateVersion7().ToString("N")[..12];
        _user = await apiFactory.IdentityTestData.SeedUser(
        [
            MenuPermissionConstants.CanManageIngredientPrices,
            MenuPermissionConstants.CanViewIngredientPrice,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanViewOffer,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanViewMenuCard,
            MenuPermissionConstants.CanEditMenuCard
        ],
        $"restaurant-scoped-menu-management-user-{_testSuffix}");

        Category productCategory = DataGenerator.CreateProductCategory($"Management ramen {_testSuffix}");
        Category ingredientCategory = DataGenerator.CreateIngredientCategory($"Management extras {_testSuffix}");
        _firstIngredient = CreateIngredient(ingredientCategory, $"PriceTest {_testSuffix} first");
        _secondIngredient = CreateIngredient(ingredientCategory, $"PriceTest {_testSuffix} second");
        Product firstProduct = CreateProduct(productCategory, $"Management active ramen {_testSuffix}");
        Product secondProduct = CreateProduct(productCategory, $"Management retired ramen {_testSuffix}");
        Product batchProductOne = CreateProduct(productCategory, $"Management batch ramen one {_testSuffix}");
        Product batchProductTwo = CreateProduct(productCategory, $"Management batch ramen two {_testSuffix}");
        Product batchProductThree = CreateProduct(productCategory, $"Management batch ramen three {_testSuffix}");

        _activeOffer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            firstProduct,
            12m);
        _retiredOffer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            secondProduct,
            15m);
        _retiredOffer.Retire();
        _batchOfferOne = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            batchProductOne,
            18m);
        _batchOfferTwo = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            batchProductTwo,
            19m);
        _batchOfferThree = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            apiFactory.RestaurantId,
            TestTenant.CompanyId,
            batchProductThree,
            20m);

        _activeCard = CreateMenuCard($"Active management card {_testSuffix}", true, _activeOffer);
        _inactiveCard = CreateMenuCard($"Inactive management card {_testSuffix}", false, _retiredOffer);
        _otherRestaurantId = Guid.CreateVersion7();
        _otherRestaurantCard = CreateMenuCard(
            $"Other restaurant card {_testSuffix}",
            false,
            null,
            _otherRestaurantId);

        MenuDbContext menuDbContext = (MenuDbContext)apiFactory.MenuDbContext;
        menuDbContext.Categories.AddRange(productCategory, ingredientCategory);
        menuDbContext.Ingredients.AddRange(_firstIngredient, _secondIngredient);
        menuDbContext.Products.AddRange(firstProduct, secondProduct, batchProductOne, batchProductTwo, batchProductThree);
        menuDbContext.RestaurantProductOffers.AddRange(
            _activeOffer,
            _retiredOffer,
            _batchOfferOne,
            _batchOfferTwo,
            _batchOfferThree);
        menuDbContext.RestaurantIngredientPrices.AddRange(
            CreatePrice(_firstIngredient, apiFactory.RestaurantId, 1.25m),
            CreatePrice(_secondIngredient, apiFactory.RestaurantId, 2.50m));
        menuDbContext.MenuCards.AddRange(_activeCard, _inactiveCard);
        await menuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using MenuDbContext otherRestaurantContext = apiFactory.CreateMenuDbContextForTenant(
            _otherRestaurantId,
            TestTenant.CompanyId);
        otherRestaurantContext.Products.Attach(firstProduct);
        otherRestaurantContext.Ingredients.Attach(_firstIngredient);
        _otherRestaurantOffer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            _otherRestaurantId,
            TestTenant.CompanyId,
            firstProduct,
            9m);
        otherRestaurantContext.RestaurantProductOffers.Add(_otherRestaurantOffer);
        otherRestaurantContext.RestaurantIngredientPrices.Add(
            CreatePrice(_firstIngredient, _otherRestaurantId, 9m));
        otherRestaurantContext.MenuCards.Add(_otherRestaurantCard);
        await otherRestaurantContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
        => await apiFactory.IdentityTestData.DeleteUser(_user.UserId);

    [Fact]
    public async Task GetIngredientPricesForManage_ShouldReturnPagedMatchesOnly_ForSelectedRestaurant()
    {
        using HttpRequestMessage request = CreateGetRequest(
            $"/api/menu/ingredient-prices/manage?ingredientName={Uri.EscapeDataString($"PriceTest {_testSuffix}")}&pageNumber=1&pageSize=1");

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);
        GetRestaurantIngredientPricesForManageResponse? result =
            await response.Content.ReadFromJsonAsync<GetRestaurantIngredientPricesForManageResponse>(
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.All(result.Items, item => Assert.Equal(apiFactory.RestaurantId, item.RestaurantId));
        Assert.False(result.AllowedActions.CanCreate);
        Assert.False(result.AllowedActions.CanEdit);
        Assert.False(result.AllowedActions.CanRemove);
    }

    [Fact]
    public async Task GetOffersForManage_ShouldIncludeRetiredAndFilterActive_ForSelectedRestaurant()
    {
        using HttpRequestMessage allOffersRequest = CreateGetRequest("/api/menu/offers/manage");

        HttpResponseMessage allOffersResponse = await apiFactory.HttpClient.SendAsync(
            allOffersRequest,
            TestContext.Current.CancellationToken);
        GetRestaurantOffersForManageResponse? allOffers =
            await allOffersResponse.Content.ReadFromJsonAsync<GetRestaurantOffersForManageResponse>(
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, allOffersResponse.StatusCode);
        Assert.NotNull(allOffers);
        Assert.Contains(allOffers.Items, offer => offer.OfferId == _retiredOffer.Id.Value);
        Assert.All(allOffers.Items, offer => Assert.Equal(apiFactory.RestaurantId, offer.RestaurantId));

        using HttpRequestMessage activeOffersRequest = CreateGetRequest("/api/menu/offers/manage?status=Active");
        HttpResponseMessage activeOffersResponse = await apiFactory.HttpClient.SendAsync(
            activeOffersRequest,
            TestContext.Current.CancellationToken);
        GetRestaurantOffersForManageResponse? activeOffers =
            await activeOffersResponse.Content.ReadFromJsonAsync<GetRestaurantOffersForManageResponse>(
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, activeOffersResponse.StatusCode);
        Assert.NotNull(activeOffers);
        Assert.Contains(activeOffers.Items, offer => offer.OfferId == _activeOffer.Id.Value);
        Assert.All(activeOffers.Items, offer => Assert.Equal("Active", offer.Status));
    }

    [Fact]
    public async Task MenuCardManagement_ShouldExposeInactiveCards_AndActivationRestoresMenuCardRead()
    {
        using HttpRequestMessage listRequest = CreateGetRequest(
            "/api/menu/cards/manage?isActive=false&pageNumber=1&pageSize=10");
        HttpResponseMessage listResponse = await apiFactory.HttpClient.SendAsync(
            listRequest,
            TestContext.Current.CancellationToken);
        GetMenuCardsForManageResponse? listResult =
            await listResponse.Content.ReadFromJsonAsync<GetMenuCardsForManageResponse>(
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotNull(listResult);
        Assert.Contains(listResult.Items, card => card.Id == _inactiveCard.Id.Value);
        Assert.DoesNotContain(listResult.Items, card => card.Id == _activeCard.Id.Value);
        Assert.DoesNotContain(listResult.Items, card => card.Id == _otherRestaurantCard.Id.Value);
        Assert.False(listResult.AllowedActions.CanCreate);
        Assert.True(listResult.AllowedActions.CanEdit);
        Assert.False(listResult.AllowedActions.CanDelete);

        using HttpRequestMessage detailRequest = CreateGetRequest(
            $"/api/menu/cards/{_inactiveCard.Id.Value}/manage");
        HttpResponseMessage detailResponse = await apiFactory.HttpClient.SendAsync(
            detailRequest,
            TestContext.Current.CancellationToken);
        GetMenuCardForManageResponse? detailResult =
            await detailResponse.Content.ReadFromJsonAsync<GetMenuCardForManageResponse>(
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.NotNull(detailResult);
        Assert.False(detailResult.IsActive);
        Assert.Equal(apiFactory.RestaurantId, detailResult.RestaurantId);
        Assert.Equal(_inactiveCard.SortOrder, detailResult.SortOrder);
        Assert.False(detailResult.AllowedActions.CanCreate);
        Assert.True(detailResult.AllowedActions.CanEdit);
        Assert.False(detailResult.AllowedActions.CanDelete);
        Assert.Contains(detailResult.EligibleOffers, offer => offer.OfferId == _activeOffer.Id.Value);
        Assert.DoesNotContain(detailResult.EligibleOffers, offer => offer.OfferId == _retiredOffer.Id.Value);
        MenuCardForManageSectionDto section = Assert.Single(detailResult.Sections);
        MenuCardForManageOfferDto offer = Assert.Single(section.Offers);
        Assert.Equal(_retiredOffer.Id.Value, offer.OfferId);
        Assert.False(offer.IsAvailable);
        Assert.Equal("Retired", offer.OfferStatus);
        Assert.Equal("Active", offer.ProductStatus);
        Assert.Equal("A valid product description for management tests.", offer.Description);
        Assert.Contains("Management ramen", offer.Categories);
        Assert.Equal(0, offer.SortOrder);
        MenuCardOfferChoiceDto eligibleOffer = Assert.Single(
            detailResult.EligibleOffers,
            choice => choice.OfferId == _activeOffer.Id.Value);
        Assert.Equal(12m, eligibleOffer.Price);
        Assert.True(eligibleOffer.IsAvailable);
        Assert.Equal("A valid product description for management tests.", eligibleOffer.Description);
        Assert.Contains("Management ramen", eligibleOffer.Categories);

        using HttpRequestMessage optionsRequest = CreateGetRequest(
            $"/api/menu/cards/options?restaurantId={apiFactory.RestaurantId}");
        HttpResponseMessage optionsResponse = await apiFactory.HttpClient.SendAsync(
            optionsRequest,
            TestContext.Current.CancellationToken);
        GetMenuCardOptionsResponse? optionsResult =
            await optionsResponse.Content.ReadFromJsonAsync<GetMenuCardOptionsResponse>(
                TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, optionsResponse.StatusCode);
        Assert.NotNull(optionsResult);
        Assert.DoesNotContain(optionsResult.MenuCards, card => card.MenuCardId == _inactiveCard.Id.Value);

        using HttpRequestMessage customerReadRequest = CreateGetRequest(
            $"/api/menu/cards/{_inactiveCard.Id.Value}");
        HttpResponseMessage inactiveCustomerResponse = await apiFactory.HttpClient.SendAsync(
            customerReadRequest,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, inactiveCustomerResponse.StatusCode);

        using HttpRequestMessage activateRequest = CreatePutRequest(
            $"/api/menu/cards/{_inactiveCard.Id.Value}/activate");
        HttpResponseMessage activateResponse = await apiFactory.HttpClient.SendAsync(
            activateRequest,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);

        using HttpRequestMessage activeCustomerReadRequest = CreateGetRequest(
            $"/api/menu/cards/{_inactiveCard.Id.Value}");
        HttpResponseMessage activeCustomerResponse = await apiFactory.HttpClient.SendAsync(
            activeCustomerReadRequest,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, activeCustomerResponse.StatusCode);

        using HttpRequestMessage activeOptionsRequest = CreateGetRequest(
            $"/api/menu/cards/options?restaurantId={apiFactory.RestaurantId}");
        HttpResponseMessage activeOptionsResponse = await apiFactory.HttpClient.SendAsync(
            activeOptionsRequest,
            TestContext.Current.CancellationToken);
        GetMenuCardOptionsResponse? activeOptionsResult =
            await activeOptionsResponse.Content.ReadFromJsonAsync<GetMenuCardOptionsResponse>(
                TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, activeOptionsResponse.StatusCode);
        Assert.NotNull(activeOptionsResult);
        Assert.Contains(activeOptionsResult.MenuCards, card => card.MenuCardId == _inactiveCard.Id.Value);
    }

    [Fact]
    public async Task UpdateMenuCardsOrder_ShouldPersistOrder_ForSelectedRestaurant()
    {
        UpdateMenuCardsOrderRequest body = new(
        [
            new MenuCardOrderItemDto(_inactiveCard.Id.Value, 0),
            new MenuCardOrderItemDto(_activeCard.Id.Value, 1)
        ]);
        using HttpRequestMessage request = CreatePutRequest("/api/menu/cards/order");
        request.WithJsonContent(body);

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);
        await response.AssertStatusCode(HttpStatusCode.NoContent);

        using HttpRequestMessage listRequest = CreateGetRequest(
            $"/api/menu/cards/manage?name={Uri.EscapeDataString($"management card {_testSuffix}")}&pageNumber=1&pageSize=10");
        HttpResponseMessage listResponse = await apiFactory.HttpClient.SendAsync(
            listRequest,
            TestContext.Current.CancellationToken);
        GetMenuCardsForManageResponse? listResult =
            await listResponse.Content.ReadFromJsonAsync<GetMenuCardsForManageResponse>(
                TestContext.Current.CancellationToken);

        await listResponse.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(listResult);
        Assert.Collection(
            listResult.Items,
            card =>
            {
                Assert.Equal(_inactiveCard.Id.Value, card.Id);
                Assert.Equal(0, card.SortOrder);
            },
            card =>
            {
                Assert.Equal(_activeCard.Id.Value, card.Id);
                Assert.Equal(1, card.SortOrder);
            });
    }

    [Fact]
    public async Task AddMenuSectionOffers_ShouldPreserveSubmittedOrderAndAppendAfterExistingItems_ForValidRequest()
    {
        MenuSection section = Assert.Single(_activeCard.Sections);
        AddMenuSectionOffersRequest body = new([_batchOfferTwo.Id.Value, _batchOfferOne.Id.Value]);
        using HttpRequestMessage request = CreatePostRequest(
            $"/api/menu/cards/{_activeCard.Id.Value}/sections/{section.Id.Value}/offers/batch");
        request.WithJsonContent(body);

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);
        await response.AssertStatusCode(HttpStatusCode.Created);
        AddMenuSectionOffersResponse? result =
            await response.Content.ReadFromJsonAsync<AddMenuSectionOffersResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Collection(
            result.Offers,
            item =>
            {
                Assert.Equal(_batchOfferTwo.Id.Value, item.OfferId);
                Assert.Equal(1, item.SortOrder);
                Assert.NotEqual(Guid.Empty, item.ListingId);
            },
            item =>
            {
                Assert.Equal(_batchOfferOne.Id.Value, item.OfferId);
                Assert.Equal(2, item.SortOrder);
                Assert.NotEqual(Guid.Empty, item.ListingId);
            });

        using HttpRequestMessage detailRequest = CreateGetRequest(
            $"/api/menu/cards/{_activeCard.Id.Value}/manage");
        HttpResponseMessage detailResponse = await apiFactory.HttpClient.SendAsync(
            detailRequest,
            TestContext.Current.CancellationToken);
        await detailResponse.AssertStatusCode(HttpStatusCode.OK);
        GetMenuCardForManageResponse? details =
            await detailResponse.Content.ReadFromJsonAsync<GetMenuCardForManageResponse>(
                TestContext.Current.CancellationToken);
        Assert.NotNull(details);
        MenuCardForManageSectionDto resultSection = Assert.Single(details.Sections);
        Assert.Collection(
            resultSection.Offers.OrderBy(item => item.SortOrder),
            item => Assert.Equal(0, item.SortOrder),
            item =>
            {
                Assert.Equal(_batchOfferTwo.Id.Value, item.OfferId);
                Assert.Equal(result.Offers[0].ListingId, item.Id);
                Assert.Equal(1, item.SortOrder);
            },
            item =>
            {
                Assert.Equal(_batchOfferOne.Id.Value, item.OfferId);
                Assert.Equal(result.Offers[1].ListingId, item.Id);
                Assert.Equal(2, item.SortOrder);
            });
    }

    [Fact]
    public async Task AddMenuSectionOffers_ShouldRejectInvalidBatchWithoutPartialPersistence()
    {
        MenuSection section = Assert.Single(_activeCard.Sections);
        Guid[][] invalidBatches =
        [
            [_batchOfferThree.Id.Value, _retiredOffer.Id.Value],
            [_batchOfferThree.Id.Value, Guid.NewGuid()],
            [_batchOfferThree.Id.Value, _activeOffer.Id.Value],
            [_batchOfferThree.Id.Value, _otherRestaurantOffer.Id.Value],
            [_batchOfferThree.Id.Value, _batchOfferThree.Id.Value],
            []
        ];

        foreach (Guid[] offerIds in invalidBatches)
        {
            using HttpRequestMessage request = CreatePostRequest(
                $"/api/menu/cards/{_activeCard.Id.Value}/sections/{section.Id.Value}/offers/batch");
            request.WithJsonContent(new AddMenuSectionOffersRequest(offerIds));
            HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
                request,
                TestContext.Current.CancellationToken);
            await response.AssertStatusCode(HttpStatusCode.BadRequest);
        }

        using HttpRequestMessage detailRequest = CreateGetRequest(
            $"/api/menu/cards/{_activeCard.Id.Value}/manage");
        HttpResponseMessage detailResponse = await apiFactory.HttpClient.SendAsync(
            detailRequest,
            TestContext.Current.CancellationToken);
        await detailResponse.AssertStatusCode(HttpStatusCode.OK);
        GetMenuCardForManageResponse? details =
            await detailResponse.Content.ReadFromJsonAsync<GetMenuCardForManageResponse>(
                TestContext.Current.CancellationToken);
        Assert.NotNull(details);
        Assert.DoesNotContain(
            Assert.Single(details.Sections).Offers,
            item => item.OfferId == _batchOfferThree.Id.Value);
    }

    [Fact]
    public async Task AddMenuSectionOffers_ShouldRejectOversizedBatch_ForMoreThanMaximumOffers()
    {
        MenuSection section = Assert.Single(_activeCard.Sections);
        Guid[] offerIds = Enumerable.Range(0, AddMenuSectionOffersValidator.MaxOfferCount + 1)
            .Select(_ => Guid.NewGuid())
            .ToArray();
        using HttpRequestMessage request = CreatePostRequest(
            $"/api/menu/cards/{_activeCard.Id.Value}/sections/{section.Id.Value}/offers/batch");
        request.WithJsonContent(new AddMenuSectionOffersRequest(offerIds));

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMenuSectionOffers_ShouldRejectForeignCardAndSection_ForSelectedRestaurant()
    {
        using HttpRequestMessage foreignCardRequest = CreatePostRequest(
            $"/api/menu/cards/{_otherRestaurantCard.Id.Value}/sections/{Guid.NewGuid()}/offers/batch");
        foreignCardRequest.WithJsonContent(new AddMenuSectionOffersRequest([_batchOfferThree.Id.Value]));

        HttpResponseMessage foreignCardResponse = await apiFactory.HttpClient.SendAsync(
            foreignCardRequest,
            TestContext.Current.CancellationToken);
        await foreignCardResponse.AssertStatusCode(HttpStatusCode.BadRequest);

        using HttpRequestMessage foreignSectionRequest = CreatePostRequest(
            $"/api/menu/cards/{_activeCard.Id.Value}/sections/{Guid.NewGuid()}/offers/batch");
        foreignSectionRequest.WithJsonContent(new AddMenuSectionOffersRequest([_batchOfferThree.Id.Value]));

        HttpResponseMessage foreignSectionResponse = await apiFactory.HttpClient.SendAsync(
            foreignSectionRequest,
            TestContext.Current.CancellationToken);
        await foreignSectionResponse.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddMenuSectionOffers_ShouldRequireEditPermission()
    {
        (string KeycloakUserId, Guid UserId) viewer = await apiFactory.IdentityTestData.SeedUser(
            [MenuPermissionConstants.CanManageMenuCards, MenuPermissionConstants.CanViewMenuCard],
            $"menu-card-batch-viewer-{Guid.CreateVersion7():N}");

        try
        {
            MenuSection section = Assert.Single(_activeCard.Sections);
            using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(
                $"/api/menu/cards/{_activeCard.Id.Value}/sections/{section.Id.Value}/offers/batch");
            request.WithJsonContent(new AddMenuSectionOffersRequest([_batchOfferThree.Id.Value]));
            request.AddAuthorizationHeader(viewer);

            HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
                request,
                TestContext.Current.CancellationToken);

            await response.AssertStatusCode(HttpStatusCode.Forbidden);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(viewer.UserId);
        }
    }

    [Fact]
    public async Task GetMenuCardForManage_ShouldRejectCardFromAnotherRestaurant()
    {
        using HttpRequestMessage request = CreateGetRequest(
            $"/api/menu/cards/{_otherRestaurantCard.Id.Value}/manage");

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ActivateMenuCard_ShouldRejectCardFromAnotherRestaurant()
    {
        using HttpRequestMessage request = CreatePutRequest(
            $"/api/menu/cards/{_otherRestaurantCard.Id.Value}/activate");

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMenuCardsForManage_ShouldReturnForbidden_ForUnassignedRestaurant()
    {
        Guid unassignedRestaurantId = await apiFactory.SeedRestaurantAsync(
            $"Unassigned Restaurant {Guid.CreateVersion7():N}");
        (string KeycloakUserId, Guid UserId) employee = await apiFactory.IdentityTestData.SeedRestaurantScopedUser(
            [MenuPermissionConstants.CanManageMenuCards, MenuPermissionConstants.CanViewMenuCard],
            [apiFactory.RestaurantId],
            $"employee-assigned-only-{Guid.CreateVersion7():N}");

        try
        {
            using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(
                "/api/menu/cards/manage?pageNumber=1&pageSize=10");
            request.AddAuthorizationHeader(employee);
            request.Headers.Add(HeaderConstants.RestaurantId, unassignedRestaurantId.ToString());

            HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
                request,
                TestContext.Current.CancellationToken);

            await response.AssertStatusCode(HttpStatusCode.Forbidden);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(employee.UserId);
        }
    }

    private HttpRequestMessage CreateGetRequest(string path)
    {
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(path);
        request.AddAuthorizationHeader(_user);
        return request;
    }

    private HttpRequestMessage CreatePutRequest(string path)
    {
        HttpRequestMessage request = new(HttpMethod.Put, path);
        request.AddAuthorizationHeader(_user);
        return request;
    }

    private HttpRequestMessage CreatePostRequest(string path)
    {
        HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(path);
        request.AddAuthorizationHeader(_user);
        return request;
    }

    private RestaurantIngredientPrice CreatePrice(Ingredient ingredient, Guid restaurantId, decimal price)
        => RestaurantIngredientPrice.Create(
            new RestaurantIngredientPriceId(Guid.CreateVersion7()),
            restaurantId,
            ingredient,
            price);

    private static Ingredient CreateIngredient(Category category, string name)
        => Ingredient.Create(
            new IngredientId(Guid.CreateVersion7()),
            TestTenant.CompanyId,
            name,
            "A valid ingredient description for management.",
            [category]);

    private static Product CreateProduct(Category category, string name)
        => Product.Create(
            new ProductId(Guid.CreateVersion7()),
            TestTenant.CompanyId,
            name,
            "A valid product description for management tests.",
            string.Empty,
            new Collection<Ingredient>(),
            new Collection<Ingredient>(),
            [category]);

    private static MenuCard CreateMenuCard(
        string name,
        bool isActive,
        RestaurantProductOffer? offer,
        Guid? restaurantId = null)
    {
        Guid cardRestaurantId = restaurantId
            ?? offer?.RestaurantId
            ?? throw new ArgumentException("Restaurant ID is required when menu card has no offer.", nameof(restaurantId));
        MenuCard card = MenuCard.Create(
            new MenuCardId(Guid.CreateVersion7()),
            cardRestaurantId,
            name);
        card.SetActive(isActive);
        if (offer is not null)
        {
            MenuSection section = card.AddSection(
                new MenuSectionId(Guid.CreateVersion7()),
                "Management section",
                0);
            section.AddOffer(
                new MenuSectionItemId(Guid.CreateVersion7()),
                offer,
                0);
        }

        return card;
    }
}
