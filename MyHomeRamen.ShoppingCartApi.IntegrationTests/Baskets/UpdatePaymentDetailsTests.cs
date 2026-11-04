using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdatePaymentDetails;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class UpdatePaymentDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
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
    public async Task UpdatePaymentDetails_ShouldPersistSelection_ForBasketIdRoute()
    {
        KeyValuePair<Guid, Guid> paymentMethod = ShoppingCartDataSet.PaymentMethods.Single();
        string basketPath = $"/api/shopping-cart/baskets/{_basket.Id.Value}";

        using HttpRequestMessage updateRequest = new(HttpMethod.Put, $"{basketPath}/payment-details");
        updateRequest.AddAuthorizationHeader(_user);
        updateRequest.WithJsonContent(new UpdatePaymentDetailsRequest(
            paymentMethod.Key.ToString(),
            paymentMethod.Value.ToString()));

        HttpResponseMessage updateResponse = await apiFactory.HttpClient.SendAsync(
            updateRequest,
            TestContext.Current.CancellationToken);

        await updateResponse.AssertStatusCode(HttpStatusCode.OK);

        using HttpRequestMessage getRequest = HttpClientExtensions.CreateGetMessage($"{basketPath}/payment-details");
        getRequest.AddAuthorizationHeader(_user);

        HttpResponseMessage getResponse = await apiFactory.HttpClient.SendAsync(
            getRequest,
            TestContext.Current.CancellationToken);

        await getResponse.AssertStatusCode(HttpStatusCode.OK);
        PaymentDetailsResponse? details = await getResponse.Content.ReadFromJsonAsync<PaymentDetailsResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.Equal(paymentMethod.Key.ToString(), details.PaymentMethodId);
        Assert.Equal(paymentMethod.Value.ToString(), details.PaymentChannelId);
    }
}
