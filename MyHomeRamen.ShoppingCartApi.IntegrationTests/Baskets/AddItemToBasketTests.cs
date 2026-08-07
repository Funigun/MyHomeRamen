using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class AddItemToBasketTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/basket/items";
    private Basket _guestBasket = default!;
    private Basket _customerBasket = default!;
    private BasketItem _guestBasketItem = default!;
    private BasketItem _customerBasketItem = default!;

    public async ValueTask InitializeAsync()
    {
        Guid guestId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(true, TestContext.Current.CancellationToken);
        Guid customerId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(false, TestContext.Current.CancellationToken);
        User? guestUser = await apiFactory.ShoppingCartDbContext.User.Specification().ByIdAsync(guestId, TestContext.Current.CancellationToken);
        User? customerUser = await apiFactory.ShoppingCartDbContext.User.Specification().ByIdAsync(customerId, TestContext.Current.CancellationToken);

        _guestBasket = DataGenerator.CreateBasket([], guestUser!);
        _customerBasket = DataGenerator.CreateBasket([], customerUser!);

        apiFactory.ShoppingCartDbContext.Basket.Add(_guestBasket);
        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Ingredient ingredient = DataGenerator.CreateIngredient();
        Product product = DataGenerator.CreateProduct([ingredient], []);
        _guestBasketItem = DataGenerator.CreateBasketItem(product);
        _customerBasketItem = DataGenerator.CreateBasketItem(product);
    }

    public async ValueTask DisposeAsync() => await ValueTask.CompletedTask;

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForAuthenticatedUser()
    {
        // Arrange
        AddItemToBasketRequest request = _customerBasketItem.ToAddBasketItemRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, _customerBasket.User.Id.Value.ToString());
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);

        AddItemToBasketResponse? responseBody = await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEqual(Guid.Empty, responseBody.BasketId);
        Assert.NotEqual(Guid.Empty, responseBody.BasketItemId);

        Assert.NotNull(response.Headers.Location);
        Assert.Contains(responseBody.BasketItemId.ToString(), response.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnCreated_WhenRequestIsValidForGuest()
    {
        // Arrange
        AddItemToBasketRequest request = _guestBasketItem.ToAddBasketItemRequest();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.WithGuestCookie(_guestBasket.User.Id.Value.ToString());
        httpRequest.WithJsonContent(request);

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
    [MemberData(nameof(InvalidAddItemToBasketRequests), MemberType = typeof(AddItemToBasketTests))]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenRequestIsInvalid(AddItemToBasketRequest request)
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(true, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<AddItemToBasketRequest> InvalidAddItemToBasketRequests()
    {
        Guid productId = Guid.NewGuid();

        BasketIngredientDto baseIngredient = DataGenerator.CreateIngredient().ToBasketIngredientDto();
        BasketIngredientDto customIngredient = DataGenerator.CreateIngredient().ToBasketIngredientDto();

        List<BasketIngredientDto> validBaseIngredients = [baseIngredient];
        List<BasketIngredientDto> validCustomIngredients = [customIngredient];

        string tooLongComment = new('a', BasketConstants.MaxCommentLength + 1);

        return
        [
            // Quantity: below minimum
            new AddItemToBasketRequest(productId, BasketConstants.MinQuantity - 1, validBaseIngredients, validCustomIngredients, null),

            // Quantity: above maximum
            new AddItemToBasketRequest(productId, BasketConstants.MaxQuantity + 1, validBaseIngredients, validCustomIngredients, null),

            // ProductId: empty
            new AddItemToBasketRequest(Guid.Empty, 1, validBaseIngredients, validCustomIngredients, null),

            // Comment: too long
            new AddItemToBasketRequest(productId, 1, validBaseIngredients, validCustomIngredients, tooLongComment),
        ];
    }

    [Fact]
    public async Task AddItemToBasket_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.User.Query().GetUserIdAsync(true, TestContext.Current.CancellationToken);

        AddItemToBasketRequest request = new(
            Guid.NewGuid(),
            1,
            [],
            [],
            null);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage(EndpointBase);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await client.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
