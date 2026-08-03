using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Baskets;

public sealed class GetCurrentBasketDetailsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/shoppingcart/basket/details";
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

        Product guestProduct = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], []);
        Product customerProduct = DataGenerator.CreateProduct([DataGenerator.CreateIngredient()], [DataGenerator.CreateIngredient()]);  
        _guestBasketItem = DataGenerator.CreateBasketItem(guestProduct);
        _customerBasketItem = DataGenerator.CreateBasketItem(customerProduct);

        _guestBasket = DataGenerator.CreateBasket([_guestBasketItem], guestUser!);
        _customerBasket = DataGenerator.CreateBasket([_customerBasketItem], customerUser!);

        apiFactory.ShoppingCartDbContext.Basket.Add(_guestBasket);
        apiFactory.ShoppingCartDbContext.Basket.Add(_customerBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _guestBasket.CheckOut();
        _customerBasket.CheckOut();
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnOk_WhenAuthenticatedUserHasActiveBasket()
    {
        // Arrange
        UserId userId = _customerBasket.User.Id;

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketDetailsResponse? details = await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.Equal(_customerBasket.Id.Value, details.BasketId);
        Assert.NotEmpty(details.Items);

        BasketDetailsItemDto firstItem = details.Items.First();
        Assert.NotEqual(Guid.Empty, firstItem.Id);
        Assert.NotNull(firstItem.Product);
        Assert.False(string.IsNullOrWhiteSpace(firstItem.Product.Name));
        Assert.False(string.IsNullOrWhiteSpace(firstItem.Product.Description));
        Assert.NotNull(firstItem.Product.BaseIngredients);
        Assert.NotNull(firstItem.Product.CustomIngredients);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnOk_WhenGuestHasActiveBasket()
    {
        // Arrange
        UserId guestId = _guestBasket.User.Id;

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.WithGuestCookie(guestId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketDetailsResponse? details = await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.Equal(_guestBasket.Id.Value, details.BasketId);
        Assert.NotEmpty(details.Items);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnNotFound_WhenUserHasNoActiveBasket()
    {
        // Arrange
        User userWithoutBasket = User.Create(new UserId(Guid.CreateVersion7()), [], [], isGuest: false);
        apiFactory.ShoppingCartDbContext.User.Add(userWithoutBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.AddAuthorizationHeader(UserRoles.Customer, userWithoutBasket.Id.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnNotFound_WhenUserIdIsEmpty()
    {
        // Arrange
        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnCorrectItemShape_WhenBasketContainsItemsWithBaseAndCustomIngredients()
    {
        // Arrange
        BasketItem basketItem = _customerBasket.Items.First();
        Product product = basketItem.Product;
        Ingredient baseIngredient = product.BaseIngredients.First();
        Ingredient customIngredient = product.CustomIngredients.First();

        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);
        request.AddAuthorizationHeader(UserRoles.Customer, _customerBasket.User.Id.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketDetailsResponse? details = await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        BasketDetailsItemDto item = details.Items.Single();

        Assert.Equal(basketItem.Id.Value, item.Id);
        Assert.Equal(basketItem.Quantity, item.Quantity);
        Assert.Equal(basketItem.Price, item.Price);
        Assert.Equal(basketItem.Comment, item.Comment);

        Assert.Equal(product.Id.Value, item.Product.Id);
        Assert.Equal(product.Name, item.Product.Name);
        Assert.Equal(product.Description, item.Product.Description);
        Assert.Equal(product.ImageUrl, item.Product.ImageUrl);

        BasketDetailsIngredientDto baseIngredientDto = item.Product.BaseIngredients.Single();
        Assert.Equal(baseIngredient.Id.Value, baseIngredientDto.Id);
        Assert.Equal(baseIngredient.Name, baseIngredientDto.Name);

        BasketDetailsIngredientDto customIngredientDto = item.Product.CustomIngredients.Single();
        Assert.Equal(customIngredient.Id.Value, customIngredientDto.Id);
        Assert.Equal(customIngredient.Name, customIngredientDto.Name);
    }
}
