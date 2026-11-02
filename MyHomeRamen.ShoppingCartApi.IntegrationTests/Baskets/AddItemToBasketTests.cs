using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class AddItemToBasketTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/basket/items";
    private Basket _guestBasket = default!;
    private Basket _customerBasket = default!;
    private BasketItem _guestBasketItem = default!;
    private BasketItem _customerBasketItem = default!;
    private (Guid UserId, Guid GuestId) _guest;
    private (Guid UserId, Guid GuestId) _customer;

    public async ValueTask InitializeAsync()
    {
        _guest = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);
        _customer = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);

        _guestBasket = DataGenerator.CreateBasket([], _guest.UserId);
        _customerBasket = DataGenerator.CreateBasket([], _customer.UserId);

        apiFactory.ShoppingCartDbContext.Basket.Add(_guestBasket);
        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Ingredient ingredient = DataGenerator.CreateIngredient();
        Product product = DataGenerator.CreateProduct([ingredient], []);
        _guestBasketItem = DataGenerator.CreateBasketItem(product);
        _customerBasketItem = DataGenerator.CreateBasketItem(product);
    }

    public async ValueTask DisposeAsync() => await ValueTask.CompletedTask;

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForAuthenticatedUser()
    {
        // Arrange
        AddItemToBasketRequest request = _customerBasketItem.ToAddBasketItemRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(_customer);
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEqual(Guid.Empty, responseBody.BasketId);
        Assert.NotEqual(Guid.Empty, responseBody.BasketItemId);

        Assert.NotNull(response.Headers.Location);
        Assert.Contains(responseBody.BasketItemId.ToString(), response.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);

        using HttpRequestMessage detailsRequest = HttpClientExtensions.CreateGetMessage("/api/shoppingcart/basket/details");
        detailsRequest.AddAuthorizationHeader(_customer);
        HttpResponseMessage detailsResponse = await client.SendAsync(detailsRequest, TestContext.Current.CancellationToken);
        await detailsResponse.AssertStatusCode(HttpStatusCode.OK);
        GetCurrentBasketDetailsResponse? details = await detailsResponse.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(details);
        BasketDetailsItemDto addedItem = Assert.Single(details.Items);
        Assert.True(addedItem.IsAvailable);
        Assert.False(addedItem.IsPriceChanged);
        Assert.Equal(addedItem.Price, addedItem.CurrentPrice);
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForGuest()
    {
        // Arrange
        AddItemToBasketRequest request = _guestBasketItem.ToAddBasketItemRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.WithGuestCookie(_guest.GuestId.ToString());
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEqual(Guid.Empty, responseBody.BasketId);
        Assert.NotEqual(Guid.Empty, responseBody.BasketItemId);
    }

    [Theory]
    [MemberData(nameof(InvalidAddItemToBasketRequests), MemberType = typeof(AddItemToBasketTests))]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenRequestIsInvalid(AddItemToBasketRequest request)
    {
        // Arrange
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(user);
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<AddItemToBasketRequest> InvalidAddItemToBasketRequests()
    {
        string tooLongComment = new('a', BasketConstants.MaxCommentLength + 1);
        Guid validRestaurantId = ShoppingCartDataSet.RestaurantId;
        Guid validOfferId = ShoppingCartDataSet.OfferId;

        return
        [
            // Quantity: below minimum
            new AddItemToBasketRequest(validRestaurantId, validOfferId, BasketConstants.MinQuantity - 1, [], null),

            // Quantity: above maximum
            new AddItemToBasketRequest(validRestaurantId, validOfferId, BasketConstants.MaxQuantity + 1, [], null),

            // RestaurantId: empty
            new AddItemToBasketRequest(Guid.Empty, validOfferId, 1, [], null),

            // OfferId: empty
            new AddItemToBasketRequest(validRestaurantId, Guid.Empty, 1, [], null),

            // Comment: too long
            new AddItemToBasketRequest(validRestaurantId, validOfferId, 1, [], tooLongComment),
        ];
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);

        AddItemToBasketRequest request = new(
            ShoppingCartDataSet.RestaurantId,
            Guid.NewGuid(),
            1,
            [],
            null);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(user);
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddItemToBasket_ShouldRequireConfirmation_WhenRestaurantChanges()
    {
        // Arrange
        AddItemToBasketRequest request = new(
            ShoppingCartDataSet.OtherRestaurantId,
            ShoppingCartDataSet.OfferId,
            1,
            [],
            null);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(_customer);
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.True(responseBody.RequiresBasketReplacement);
        Assert.Equal(ShoppingCartDataSet.RestaurantId, responseBody.CurrentRestaurantId);
        Assert.Equal(_customerBasket.Id.Value, responseBody.BasketId);

        Basket? unchangedBasket = await apiFactory.ShoppingCartDbContext.Basket.Load()
            .GetForUserTrackedAsync(new UserId(_customer.UserId), TestContext.Current.CancellationToken);
        Assert.NotNull(unchangedBasket);
        Assert.Equal(ShoppingCartDataSet.RestaurantId, unchangedBasket.RestaurantId);
        Assert.Empty(unchangedBasket.Items);

        using HttpRequestMessage confirmedRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        confirmedRequest.AddAuthorizationHeader(_customer);
        confirmedRequest.WithJsonContent(request with { ReplaceExistingBasket = true });
        HttpResponseMessage confirmedResponse = await client.SendAsync(confirmedRequest, TestContext.Current.CancellationToken);
        await confirmedResponse.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? confirmedBody = await confirmedResponse.Content.ReadFromJsonAsync<AddItemToBasketResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(confirmedBody);
        Assert.NotEqual(_customerBasket.Id.Value, confirmedBody.BasketId);

        Basket? replacedBasket = await apiFactory.ShoppingCartDbContext.Basket.Load()
            .GetForUserTrackedAsync(new UserId(_customer.UserId), TestContext.Current.CancellationToken);
        Assert.NotNull(replacedBasket);
        Assert.Equal(ShoppingCartDataSet.OtherRestaurantId, replacedBasket.RestaurantId);
        Assert.Single(replacedBasket.Items);
    }
}
