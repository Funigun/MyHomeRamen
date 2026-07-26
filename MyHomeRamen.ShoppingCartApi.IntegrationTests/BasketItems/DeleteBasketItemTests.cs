using System.Net;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.BasketItems;
/*
public sealed class DeleteBasketItemTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shoppingcart/baskets";

    private static string BuildUrl(Guid basketId, Guid basketItemId)
        => $"{EndpointBase}/{basketId}/items/{basketItemId}";

    [Fact]
    public async Task DeleteBasketItem_ShouldReturnNoContent_ForValidIds()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        BasketItem item = basket.Items.First();
        UserId userId = basket.User.Id;

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions
            .CreateDeleteMessage(BuildUrl(basket.Id.Value, item.Id.Value))
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBasketItem_ShouldReturnBadRequest_ForBasketNotOwnedByUser()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        BasketItem item = basket.Items.First();
        Guid differentUserId = Guid.NewGuid();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions
            .CreateDeleteMessage(BuildUrl(basket.Id.Value, item.Id.Value))
            .AddAuthorizationHeader(UserRoles.Customer, differentUserId.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBasketItem_ShouldReturnBadRequest_ForNonExistentBasketItem()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        UserId userId = basket.User.Id;
        Guid nonExistentItemId = Guid.NewGuid();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions
            .CreateDeleteMessage(BuildUrl(basket.Id.Value, nonExistentItemId))
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBasketItem_ShouldReturnBadRequest_ForEmptyBasketId()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        BasketItem item = basket.Items.First();
        UserId userId = basket.User.Id;

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions
            .CreateDeleteMessage(BuildUrl(Guid.Empty, item.Id.Value))
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBasketItem_ShouldReturnBadRequest_ForEmptyBasketItemId()
    {
        // Arrange
        Basket basket = DataGenerator.GeneratedBaskets.First();
        UserId userId = basket.User.Id;

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions
            .CreateDeleteMessage(BuildUrl(basket.Id.Value, Guid.Empty))
            .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
*/
