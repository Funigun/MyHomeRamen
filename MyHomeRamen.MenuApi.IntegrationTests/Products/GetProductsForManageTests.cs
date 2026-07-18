using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class GetProductsForManageTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string Endpoint = "/api/menu/products/manage";
    private Product _product = default!;

    public async ValueTask InitializeAsync()
    {
        _product = DataGenerator.GeneratedProducts.First();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetProductsForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetProductsForManage_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByName()
    {
        // Arrange
        string partialName = _product.Name[..5];

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{Endpoint}?name={Uri.EscapeDataString(partialName)}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.All(result.Products, p => Assert.Contains(partialName, p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByCategoryId()
    {
        // Arrange
        Guid categoryId = _product.Categories.First().Id.Value;

        IEnumerable<Guid> expectedProductIds = DataGenerator.GeneratedProducts
            .Where(p => p.Categories.Any(c => c.Id.Value == categoryId))
            .Select(p => p.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{Endpoint}?categoryIds={categoryId}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
        Assert.All(result.Products, p => Assert.Contains(p.Id, expectedProductIds));
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByIngredientId()
    {
        // Arrange
        Guid ingredientId = _product.BaseIngredients.First().Id.Value;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{Endpoint}?ingredientIds={ingredientId}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
        Assert.Contains(result.Products, p => p.Id == _product.Id.Value);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByPriceRange()
    {
        // Arrange
        decimal priceFrom = _product.Price - 1m;
        decimal priceTo = _product.Price + 1m;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{Endpoint}?priceFrom={priceFrom}&priceTo={priceTo}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.All(result.Products, p =>
        {
            Assert.True(p.Price >= priceFrom);
            Assert.True(p.Price <= priceTo);
        });
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnPagedResults()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{Endpoint}?pageSize=1")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.True(result.TotalCount > 1);
        Assert.Single(result.Products);
    }
}
