using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class GetCategoriesForManageTests(WebApiFactory apiFactory)
{
    private const string Endpoint = "/api/menu/categories/manage";

    [Fact]
    public async Task GetCategoriesForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesForManage_ShouldReturnBothNonEmptyLists_ForSeededData()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetCategoriesForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotNull(result.ProductCategories);
        Assert.NotEmpty(result.ProductCategories);
        Assert.NotNull(result.IngredientCategories);
        Assert.NotEmpty(result.IngredientCategories);
    }

    [Fact]
    public async Task GetCategoriesForManage_ShouldReturnProductCategoriesOrderedBySortOrder()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetCategoriesForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotNull(result.ProductCategories);

        List<int> sortOrders = result.ProductCategories.Select(c => c.SortOrder).ToList();
        List<int> expectedSortOrders = sortOrders.OrderBy(s => s).ToList();

        Assert.Equal(expectedSortOrders, sortOrders);
    }

    [Fact]
    public async Task GetCategoriesForManage_ShouldReturnIngredientCategoriesOrderedBySortOrder()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetCategoriesForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotNull(result.IngredientCategories);

        List<int> sortOrders = result.IngredientCategories.Select(c => c.SortOrder).ToList();
        List<int> expectedSortOrders = sortOrders.OrderBy(s => s).ToList();

        Assert.Equal(expectedSortOrders, sortOrders);
    }

    [Fact]
    public async Task GetCategoriesForManage_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetCategoriesForManage_ShouldReturnForbidden_ForNonManagerRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage(Endpoint)
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }
}
