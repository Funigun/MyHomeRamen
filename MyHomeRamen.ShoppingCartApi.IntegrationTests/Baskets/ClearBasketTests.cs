using System.Net;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class ClearBasketTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/baskets";
  
    private Basket _customerBasket = default!;
    private (Guid UserId, Guid GuestId) _customer;

    public async ValueTask InitializeAsync()
    {
        _customer = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanRemoveProduct]);
 
        _customerBasket = DataGenerator.CreateBasket([], _customer.UserId);

        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await ValueTask.CompletedTask;

    [Fact]
    public async Task ClearBasket_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        string endpoint = $"{EndpointBase}/{_customerBasket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(_customer);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForNonExistentBasket()
    {
        // Arrange
        string endpoint = $"{EndpointBase}/{Guid.NewGuid()}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(_customer);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ClearBasket_ShouldReturnBadRequest_ForBasketBelongingToDifferentUser()
    {
        // Arrange
        (Guid UserId, Guid GuestId) differentUser = await apiFactory.IdentityTestData.SeedGuest([PermissionConstants.CanRemoveProduct]);
        string endpoint = $"{EndpointBase}/{_customerBasket.Id.Value}";

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateDeleteMessage(endpoint);
        request.AddAuthorizationHeader(differentUser);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
