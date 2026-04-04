using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class GetCategoriesByTypeTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/menu/categories/by-type";

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOkWithList_ForIngredientType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Ingredient}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetCategoriesByTypeResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOk_ForAuthenticatedManager()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetCategoriesByType_ShouldReturnForbidden_ForNonManagerRoles(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnBadRequest_ForInvalidCategoryType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}?categoryType=999")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
    }
}
