using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Products;

public sealed class GetProductsForManageTests(WebApiFactory apiFactory)
{
    private const string Endpoint = "/api/menu/products/manage";

    [Fact]
    public async Task GetProductsForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

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
        Product product = DataGenerator.GeneratedProducts.First();
        string partialName = product.Name[..5];

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
        Product product = DataGenerator.GeneratedProducts.First();
        Guid categoryId = product.Categories.First().Id.Value;

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
        Product product = DataGenerator.GeneratedProducts.First();
        Guid ingredientId = product.BaseIngredients.First().Id.Value;

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
        Assert.Contains(result.Products, p => p.Id == product.Id.Value);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByPriceRange()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();
        decimal priceFrom = product.Price - 1m;
        decimal priceTo = product.Price + 1m;

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
