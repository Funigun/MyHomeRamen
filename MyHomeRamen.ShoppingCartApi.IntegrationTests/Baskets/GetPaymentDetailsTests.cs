using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using System.Text;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class GetPaymentDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    private static readonly CompositeFormat EndpointBase = CompositeFormat.Parse("/api/shopping-cart/{0}/payment-details");

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnOk_ForBasketWithPaymentDetails()
    {
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanCheckout]);

        PaymentDetails paymentDetails = ShoppingCartDataSet.CashPaymentDetails();
        Product product = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], []);
        Basket basket = DataGenerator.CreateBasket([DataGenerator.CreateBasketItem(product)], user.UserId, paymentDetails: paymentDetails);

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(user);

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.OK);
        PaymentDetailsResponse? responseBody = await response.Content.ReadFromJsonAsync<PaymentDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.Equal(paymentDetails.PaymentMethodId, responseBody.PaymentMethodId);
        Assert.Equal(paymentDetails.PaymentChannelId, responseBody.PaymentChannelId);
    }

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnBadRequest_ForNonExistentBasket()
    {
        (Guid UserId, Guid GuestId) user = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanCheckout]);

        string url = string.Format(null, EndpointBase, Guid.NewGuid());

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(user);

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnBadRequest_ForBasketOfAnotherUser()
    {
        (Guid UserId, Guid GuestId) customer = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanCheckout]);
        (Guid UserId, Guid GuestId) otherUser = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanCheckout]);

        Ingredient ingredient = DataGenerator.CreateIngredient();
        Product product = DataGenerator.CreateProduct([ingredient], []);
        BasketItem basketItem = DataGenerator.CreateBasketItem(product);
        Basket? basket = DataGenerator.CreateBasket([basketItem], customer.UserId);

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(otherUser);

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
