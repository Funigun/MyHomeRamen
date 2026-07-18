using System.Net;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Baskets;

public sealed class ClearBasketTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task ClearBasket_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        UserId userId = basket.User.Id;
        string endpoint = $"/api/shoppingcart/baskets/{basket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint)
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForNonExistentBasket()
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);
        string endpoint = $"/api/shoppingcart/baskets/{Guid.NewGuid()}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint)
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForBasketBelongingToDifferentUser()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        UserId differentUserId = new(Guid.NewGuid());
        string endpoint = $"/api/shoppingcart/baskets/{basket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint)
            .AddAuthorizationHeader(UserRoles.Customer, differentUserId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
