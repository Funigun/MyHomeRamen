using System.Net;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.CheckoutBasket;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class CheckoutBasketTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string Endpoint = "/api/shopping-cart/basket/checkout";
    private Guid _userId;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (_userId != Guid.Empty)
        {
            await apiFactory.IdentityTestData.DeleteUser(_userId);
        }
    }

    [Fact]
    public async Task CheckoutBasket_ShouldReturnBadRequest_ForEmptyRestaurantIdOnCanonicalRoute()
    {
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);
        _userId = user.UserId;

        using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(Endpoint);
        request.AddAuthorizationHeader(user);
        request.WithJsonContent(new CheckoutBasketRequest(Guid.Empty, "USD"));

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckoutBasket_ShouldRequireSingleRestaurantHeader()
    {
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);
        _userId = user.UserId;

        using HttpClient client = apiFactory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderConstants.RestaurantId);
        using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(Endpoint);
        request.AddAuthorizationHeader(user);
        request.Headers.Add(HeaderConstants.RestaurantId, ShoppingCartDataSet.RestaurantId.ToString());
        request.Headers.Add(HeaderConstants.RestaurantId, ShoppingCartDataSet.OtherRestaurantId.ToString());
        request.WithJsonContent(new CheckoutBasketRequest(ShoppingCartDataSet.RestaurantId, "USD"));

        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckoutBasket_ShouldRejectBodyRestaurantDifferentFromHeader()
    {
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);
        _userId = user.UserId;

        using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(Endpoint);
        request.AddAuthorizationHeader(user);
        request.WithJsonContent(new CheckoutBasketRequest(ShoppingCartDataSet.OtherRestaurantId, "USD"));

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckoutBasket_ShouldNotFindLegacyRoute()
    {
        using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage("/api/shoppingcart/basket/checkout");
        request.WithJsonContent(new CheckoutBasketRequest(Guid.Empty, "USD"));

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }
}
