using System.Net;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class ClearBasketTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/baskets";
  
    private Basket _customerBasket = default!;

    public async ValueTask InitializeAsync()
    {
        Guid customerId = Guid.CreateVersion7();
 
        _customerBasket = DataGenerator.CreateBasket([], customerId);

        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await ValueTask.CompletedTask;

    [Fact]
    public async Task ClearBasket_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        UserId userId = _customerBasket.UserId;
        string endpoint = $"{EndpointBase}/{_customerBasket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForNonExistentBasket()
    {
        // Arrange
        UserId userId = _customerBasket.UserId;
        string endpoint = $"{EndpointBase}/{Guid.NewGuid()}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForBasketBelongingToDifferentUser()
    {
        // Arrange
        UserId differentUserId = new(Guid.NewGuid());
        string endpoint = $"{EndpointBase}/{_customerBasket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(UserRoles.Customer, differentUserId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
