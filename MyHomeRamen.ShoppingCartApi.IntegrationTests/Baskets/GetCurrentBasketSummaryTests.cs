using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class GetCurrentBasketSummaryTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/basket/summary";
    private Basket _guestBasket = default!;
    private Basket _customerBasket = default!;
    private BasketItem _guestBasketItem = default!;
    private BasketItem _customerBasketItem = default!;
    private (Guid UserId, Guid GuestId) _guest;
    private (Guid UserId, Guid GuestId) _customer;

    public async ValueTask InitializeAsync()
    {
        _guest = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanViewBasket]);
        _customer = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanViewBasket]);

        Ingredient ingredient = DataGenerator.CreateIngredient();
        Product product = DataGenerator.CreateProduct([ingredient], []);
        _guestBasketItem = DataGenerator.CreateBasketItem(product);
        _customerBasketItem = DataGenerator.CreateBasketItem(product);

        _guestBasket = DataGenerator.CreateBasket([_guestBasketItem], _guest.UserId);
        _customerBasket = DataGenerator.CreateBasket([_customerBasketItem], _customer.UserId);

        apiFactory.ShoppingCartDbContext.Basket.Add(_guestBasket);
        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _guestBasket.CheckOut();
        _customerBasket.CheckOut();
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnOk_ForAnyAuthenticatedRole()
    {
        // Arrange
        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.AddAuthorizationHeader(_customer);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        Assert.Equal(_customerBasket.Id.Value, getResponse.Id);
        Assert.NotEmpty(getResponse.Items);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnBasketWithCorrectItems_ForAuthenticatedUser()
    {
        // Arrange
        Basket expectedBasket = _customerBasket;
        BasketItem expectedItem = expectedBasket.Items.First();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.AddAuthorizationHeader(_customer);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        BasketSummaryItemDto actualItem = getResponse.Items.First(i => i.Id == expectedItem.Id.Value);

        Assert.Equal(expectedItem.Product.Name, actualItem.ProductName);
        Assert.Equal(expectedItem.Product.ImageUrl, actualItem.ProductImageUrl);
        Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
        Assert.Equal(expectedItem.Price, actualItem.Price);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnOk_ForGuest()
    {
        // Arrange
        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.WithGuestCookie(_guest.GuestId.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        Assert.Equal(_guestBasket.Id.Value, getResponse.Id);
        Assert.NotEmpty(getResponse.Items);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnBasketWithCorrectItems_ForGuest()
    {
        // Arrange
        Basket expectedBasket = _guestBasket;
        BasketItem expectedItem = expectedBasket.Items.First();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.WithGuestCookie(_guest.GuestId.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        BasketSummaryItemDto actualItem = getResponse.Items.First(i => i.Id == expectedItem.Id.Value);

        Assert.Equal(expectedItem.Product.Name, actualItem.ProductName);
        Assert.Equal(expectedItem.Product.ImageUrl, actualItem.ProductImageUrl);
        Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
        Assert.Equal(expectedItem.Price, actualItem.Price);
    }
}
