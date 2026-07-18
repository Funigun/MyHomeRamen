using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Baskets;

public sealed class GetPaymentDetailsTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shopping-cart/{0}/payment-details";

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnOk_ForBasketWithPaymentDetails()
    {
        ShoppingCartDbContext? context = apiFactory.ShoppingCartDbContext;
        User? user = await context.Users.FirstAsync(u => !u.IsGuest, TestContext.Current.CancellationToken);

        List<Product>? products = await context.Products.Take(1).ToListAsync(TestContext.Current.CancellationToken);
        Basket? basket = DataGenerator.GenerateValidBasket(user, products);
        PaymentDetails? paymentDetails = PaymentDetails.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        typeof(Basket).GetProperty(nameof(Basket.PaymentDetails))!.SetValue(basket, paymentDetails);

        context.ShoppingCarts.Add(basket);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url)
                                                                  .AddAuthorizationHeader(UserRoles.Customer, user.Id.Value.ToString());

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
        ShoppingCartDbContext? context = apiFactory.ShoppingCartDbContext;
        User? user = await context.Users.FirstAsync(u => !u.IsGuest, TestContext.Current.CancellationToken);

        string url = string.Format(EndpointBase, Guid.NewGuid());

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url)
                                                                  .AddAuthorizationHeader(UserRoles.Customer, user.Id.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaymentDetails_ShouldReturnBadRequest_ForBasketOfAnotherUser()
    {
        ShoppingCartDbContext? context = apiFactory.ShoppingCartDbContext;
        List<User>? users = await context.Users.Where(u => !u.IsGuest).Take(2).ToListAsync(TestContext.Current.CancellationToken);
        User? basketOwner = users[0];
        User? otherUser = users[1];

        List<Product>? products = await context.Products.Take(1).ToListAsync(TestContext.Current.CancellationToken);
        Basket? basket = DataGenerator.GenerateValidBasket(basketOwner, products);

        context.ShoppingCarts.Add(basket);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url)
                                                                  .AddAuthorizationHeader(UserRoles.Customer, otherUser.Id.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
