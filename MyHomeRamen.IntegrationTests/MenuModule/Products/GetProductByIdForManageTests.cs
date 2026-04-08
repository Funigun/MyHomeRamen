using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Products;

public sealed class GetProductByIdForManageTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/menu/products";

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{product.Id.Value}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(product.Id.Value, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
    }

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetProductByIdForManage_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{product.Id.Value}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetProductByIdForManage_ResponseShouldContainCategoryAndIngredientIds()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First(p => p.Categories.Count > 0 && p.BaseIngredients.Count > 0);
        Guid expectedCategoryId = product.Categories.First().Id.Value;
        IEnumerable<Guid> expectedIngredientIds = product.BaseIngredients.Select(i => i.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{product.Id.Value}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(expectedCategoryId, result.CategoryId);
        Assert.Equal(expectedIngredientIds.OrderBy(id => id), result.IngredientIds.OrderBy(id => id));
    }
}
