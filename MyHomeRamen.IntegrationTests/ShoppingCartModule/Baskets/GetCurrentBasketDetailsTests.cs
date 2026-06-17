using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Baskets;

public sealed class GetCurrentBasketDetailsTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shoppingcart/basket/summary";

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnOk_WhenAuthenticatedUserHasActiveBasket()
    {
        // Arrange
        UserId userId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase)
                                                        .AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketDetailsResponse? details = await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.Equal(DataGenerator.GeneratedBaskets.First().Id.Value, details.BasketId);
        Assert.NotEmpty(details.Items);

        BasketDetailsItemDto firstItem = details.Items.First();
        Assert.NotEqual(Guid.Empty, firstItem.Id);
        Assert.NotNull(firstItem.Product);
        Assert.False(string.IsNullOrWhiteSpace(firstItem.Product.Name));
        Assert.False(string.IsNullOrWhiteSpace(firstItem.Product.Description));
        Assert.False(string.IsNullOrWhiteSpace(firstItem.Product.ImageUrl));
        Assert.NotNull(firstItem.Product.BaseIngredients);
        Assert.NotNull(firstItem.Product.CustomIngredients);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnOk_WhenGuestHasActiveBasket()
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(true, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase)
                                                        .WithGuestCookie(guestId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketDetailsResponse? details = await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(details);
        Assert.Equal(DataGenerator.GeneratedBaskets.Skip(1).First().Id.Value, details.BasketId);
        Assert.NotEmpty(details.Items);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnNotFound_WhenUserHasNoActiveBasket()
    {
        // Arrange
        User userWithoutBasket = User.Create(new UserId(Guid.CreateVersion7()), [], [], isGuest: false);
        apiFactory.ShoppingCartDbContext.Users.Add(userWithoutBasket);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase)
                                                        .AddAuthorizationHeader(UserRoles.Customer, userWithoutBasket.Id.Value.ToString());

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
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase);

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentBasketDetails_ShouldReturnCorrectItemShape_WhenBasketContainsItemsWithBaseAndCustomIngredients()
    {
        // Arrange
        User user = User.Create(new UserId(Guid.CreateVersion7()), [], [], isGuest: false);

        Ingredient baseIngredient = Ingredient.Create(
            new IngredientId(Guid.NewGuid()),
            new IngredientId(Guid.NewGuid()),
            "Base Ingredient",
            "Base ingredient description",
            1.5m,
            1);

        Ingredient customIngredient = Ingredient.Create(
            new IngredientId(Guid.NewGuid()),
            new IngredientId(Guid.NewGuid()),
            "Custom Item",
            "Custom ingredient description",
            2.0m,
            1);

        Product product = Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            "Special Ramen Product",
            "A very long ramen product description that satisfies domain validators.",
            25.0m,
            "https://example.com/ramen.jpg",
            [baseIngredient],
            [customIngredient]);

        BasketItem basketItem = BasketItem.Create(new BasketItemId(Guid.NewGuid()), product, 2, "No extra spice");

        Basket basket = Basket.Create(new BasketId(Guid.NewGuid()), user);
        AddItemsToBasket(basket, basketItem);

        apiFactory.ShoppingCartDbContext.Users.Add(user);
        apiFactory.ShoppingCartDbContext.Ingredients.AddRange(baseIngredient, customIngredient);
        apiFactory.ShoppingCartDbContext.Products.Add(product);
        apiFactory.ShoppingCartDbContext.ShoppingCarts.Add(basket);
        apiFactory.ShoppingCartDbContext.BasketItems.Add(basketItem);
        await apiFactory.ShoppingCartDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase)
                                                        .AddAuthorizationHeader(UserRoles.Customer, user.Id.Value.ToString());

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

    private static void AddItemsToBasket(Basket basket, params BasketItem[] items)
    {
        System.Reflection.FieldInfo basketItemsField = typeof(Basket).GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        List<BasketItem> currentItems = (List<BasketItem>)basketItemsField.GetValue(basket)!;
        currentItems.AddRange(items);
    }
}
