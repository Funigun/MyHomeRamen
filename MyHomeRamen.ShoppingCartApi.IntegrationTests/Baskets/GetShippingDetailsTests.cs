using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;
using MyHomeRamen.Domain.ShoppingCart.Users;
using System.Text;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class GetShippingDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    private static readonly CompositeFormat EndpointBase = CompositeFormat.Parse("/api/shopping-cart/{0}/shipping-details");

    [Fact]
    public async Task GetShippingDetails_ShouldReturnOk_ForBasketWithShippingDetails()
    {
        UserId userId = Guid.CreateVersion7();

        ShippingDetails shippingDetails = ShoppingCartDataSet.DeliveryShippingDetails();
        Product product = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], []);
        Basket basket = DataGenerator.CreateBasket([DataGenerator.CreateBasketItem(product)], new UserId(userId), shippingDetails: shippingDetails);

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.OK);
        ShippingDetailsResponse? responseBody = await response.Content.ReadFromJsonAsync<ShippingDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.False(responseBody.PersonalPickup);
        Assert.True(responseBody.Delivery);
        Assert.NotNull(responseBody.ShippingAddress);
        Assert.Equal("Test street", responseBody.ShippingAddress.Street);
    }

    [Fact]
    public async Task GetShippingDetails_ShouldReturnBadRequest_ForNonExistentBasket()
    {
        UserId userId = Guid.CreateVersion7();

        string url = string.Format(null, EndpointBase, Guid.NewGuid());

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetShippingDetails_ShouldReturnBadRequest_ForBasketOfAnotherUser()
    {
        UserId basketOwnerId = Guid.CreateVersion7();
        UserId otherUserId = Guid.CreateVersion7();

        Product product = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], []);
        Basket? basket = DataGenerator.CreateBasket([DataGenerator.CreateBasketItem(product)], new UserId(basketOwnerId));

        apiFactory.ShoppingCartDbContext.Basket.Add(basket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string url = string.Format(null, EndpointBase, basket.Id.Value);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(url);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, otherUserId.Value.ToString());

        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
