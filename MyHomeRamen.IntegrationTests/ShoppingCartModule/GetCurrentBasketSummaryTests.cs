using System.Net.Http.Json;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary.Models;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule;

public sealed class GetCurrentBasketSummaryTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shoppingcart/baskets";

    [Theory]
    [InlineData(UserRoles.Customer)]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Admin)]
    public async Task GetCurrentBasketSummary_ShouldReturnOk_ForAnyAuthenticatedRole(UserRoles role)
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase).AddAuthorizationHeader(role, guestId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        string msg = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        Assert.Equal(DataGenerator.GeneratedBaskets.First().Id.Value, getResponse.Id);
        Assert.NotEmpty(getResponse.Items);
    }

    [Theory]
    [InlineData(UserRoles.Customer)]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Admin)]
    public async Task GetCurrentBasketSummary_ShouldReturnBasketWithCorrectItems_ForAnyAuthenticatedRole(UserRoles role)
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(false, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase).AddAuthorizationHeader(role, guestId.Value.ToString());

        Basket expectedBasket = DataGenerator.GeneratedBaskets.First();
        BasketItem expectedItem = expectedBasket.Items.First();

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        BasketItemDto actualItem = getResponse.Items.First(i => i.Id == expectedItem.Id.Value);

        Assert.Equal(expectedItem.Product.Name, actualItem.ProductName);
        Assert.Equal(expectedItem.Product.ImageUrl, actualItem.ProductImageUrl);
        Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
        Assert.Equal(expectedItem.Price, actualItem.Price);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnOk_ForGuest()
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(true, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase).WithGuestCookie(guestId.Value.ToString());

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        Assert.Equal(DataGenerator.GeneratedBaskets.Skip(1).First().Id.Value, getResponse.Id);
        Assert.NotEmpty(getResponse.Items);
    }

    [Fact]
    public async Task GetCurrentBasketSummary_ShouldReturnBasketWithCorrectItems_ForGuest()
    {
        // Arrange
        UserId guestId = await apiFactory.ShoppingCartDbContext.GetUserId(true, TestContext.Current.CancellationToken);

        using HttpClient client = apiFactory.CreateClient();
        HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(EndpointBase).WithGuestCookie(guestId.Value.ToString());
        Basket expectedBasket = DataGenerator.GeneratedBaskets.Skip(1).First();
        BasketItem expectedItem = expectedBasket.Items.First();

        // Act
        HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        GetCurrentBasketSummaryResponse? getResponse = await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getResponse);
        BasketItemDto actualItem = getResponse.Items.First(i => i.Id == expectedItem.Id.Value);

        Assert.Equal(expectedItem.Product.Name, actualItem.ProductName);
        Assert.Equal(expectedItem.Product.ImageUrl, actualItem.ProductImageUrl);
        Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
        Assert.Equal(expectedItem.Price, actualItem.Price);
    }
}
