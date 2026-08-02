using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using System.Text;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class GetPaymentDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    private static readonly CompositeFormat EndpointBase = CompositeFormat.Parse("/api/shopping-cart/{0}/payment-details");

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnOk_ForBasketWithPaymentDetails()
    {
        UserId userId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(false, TestContext.Current.CancellationToken);
        User user = (await apiFactory.ShoppingCartDbContext.User.Query().FindByIdAsync(userId, TestContext.Current.CancellationToken))!;

        PaymentDetails paymentDetails = ShoppingCartDataSet.CashPaymentDetails();
        Product product = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], []);
        Basket basket = DataGenerator.CreateBasket([DataGenerator.CreateBasketItem(product)], user!, paymentDetails: paymentDetails);

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, user.Id.Value.ToString());

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
        UserId userId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(false, TestContext.Current.CancellationToken);
        User user = (await apiFactory.ShoppingCartDbContext.User.Query().FindByIdAsync(userId, TestContext.Current.CancellationToken))!;

        string url = string.Format(null, EndpointBase, Guid.NewGuid());

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, user.Id.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnBadRequest_ForBasketOfAnotherUser()
    {
        UserId customeId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(false, TestContext.Current.CancellationToken);
        UserId guestId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(true, TestContext.Current.CancellationToken);
        User? basketOwner = (await apiFactory.ShoppingCartDbContext.User.Query().FindByIdAsync(customeId, TestContext.Current.CancellationToken))!;
        User? otherUser = (await apiFactory.ShoppingCartDbContext.User.Query().FindByIdAsync(guestId, TestContext.Current.CancellationToken))!;

        Ingredient ingredient = DataGenerator.CreateIngredient();
        Product product = DataGenerator.CreateProduct([ingredient], []);
        BasketItem basketItem = DataGenerator.CreateBasketItem(product);
        Basket? basket = DataGenerator.CreateBasket([basketItem], basketOwner);

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, otherUser.Id.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
