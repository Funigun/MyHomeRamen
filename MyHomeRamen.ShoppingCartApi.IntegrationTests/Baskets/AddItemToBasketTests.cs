using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Baskets;

public sealed class AddItemToBasketTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shoppingcart/basket/items";

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForAuthenticatedUser()
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);
        AddItemToBasketRequest request = DataGenerator.ValidAddItemToBasketRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase)
                                                                  .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString())
                                                                  .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEqual(Guid.Empty, responseBody.BasketId);
        Assert.NotEqual(Guid.Empty, responseBody.BasketItemId);

        Assert.NotNull(response.Headers.Location);
        Assert.Contains(responseBody.BasketItemId.ToString(), response.Headers.Location.ToString());
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForGuest()
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(true, TestContext.Current.CancellationToken);
        AddItemToBasketRequest request = DataGenerator.ValidAddItemToBasketRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase)
                                                                   .WithGuestCookie(guestId.Value.ToString())
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEqual(Guid.Empty, responseBody.BasketId);
        Assert.NotEqual(Guid.Empty, responseBody.BasketItemId);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidAddItemToBasketRequests), MemberType = typeof(DataGenerator))]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenRequestIsInvalid(AddItemToBasketRequest request)
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase)
                                                                   .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString())
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<AddItemToBasketRequest> InvalidAddItemToBasketRequests()
    {
        MenuProduct menuProduct = GetRandomMenuProduct();

        List<BasketIngredientDto> validBaseIngredients = menuProduct.BaseIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        List<BasketIngredientDto> validCustomIngredients = menuProduct.CustomIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        string tooLongComment = new('a', BasketItemCommentValidator.MaxCommentLength + 1);

        return
        [
            // Quantity: below minimum
            new AddItemToBasketRequest(menuProduct.Id, BasketItemQuantityValidator.MinQuantity - 1, validBaseIngredients, validCustomIngredients, null),

            // Quantity: above maximum
            new AddItemToBasketRequest(menuProduct.Id, BasketItemQuantityValidator.MaxQuantity + 1, validBaseIngredients, validCustomIngredients, null),

            // ProductId: empty
            new AddItemToBasketRequest(Guid.Empty, 1, validBaseIngredients, validCustomIngredients, null),

            // Comment: too long
            new AddItemToBasketRequest(menuProduct.Id, 1, validBaseIngredients, validCustomIngredients, tooLongComment),
        ];
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);

        AddItemToBasketRequest request = new(
            Guid.NewGuid(),
            1,
            [],
            [],
            null);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase)
                                                                   .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString())
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
