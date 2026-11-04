using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class UpdateShippingDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Basket _basket = default!;
    private (Guid UserId, Guid GuestId) _user;

    public async ValueTask InitializeAsync()
    {
        _user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.ManageBasket]);
        _basket = DataGenerator.CreateBasket([], _user.UserId);

        apiFactory.ShoppingCartDbContext.Basket.Add(_basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _basket.CheckOut();
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        await apiFactory.IdentityTestData.DeleteUser(_user.UserId);
    }

    [Fact]
    public async Task UpdateShippingDetails_ShouldPersistPickupSelection_ForBasketIdRoute()
    {
        string basketPath = $"/api/shopping-cart/baskets/{_basket.Id.Value}";

        using HttpRequestMessage updateRequest = new(HttpMethod.Put, $"{basketPath}/shipping-details");
        updateRequest.AddAuthorizationHeader(_user);
        updateRequest.WithJsonContent(new UpdateShippingDetailsRequest(true, false, null));

        HttpResponseMessage updateResponse = await apiFactory.HttpClient.SendAsync(
            updateRequest,
            TestContext.Current.CancellationToken);

        await updateResponse.AssertStatusCode(HttpStatusCode.OK);

        using HttpRequestMessage getRequest = HttpClientExtensions.CreateGetMessage($"{basketPath}/shipping-details");
        getRequest.AddAuthorizationHeader(_user);

        HttpResponseMessage getResponse = await apiFactory.HttpClient.SendAsync(
            getRequest,
            TestContext.Current.CancellationToken);

        await getResponse.AssertStatusCode(HttpStatusCode.OK);
        ShippingDetailsResponse? details = await getResponse.Content.ReadFromJsonAsync<ShippingDetailsResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.True(details.PersonalPickup);
        Assert.False(details.Delivery);
        Assert.Null(details.ShippingAddress);
    }
}
