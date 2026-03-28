using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class GetCategoriesForDropdownTests(WebApiFactory apiFactory)
{
    [Theory]
    [InlineData((int)CategoryType.Product)]
    [InlineData((int)CategoryType.Ingredient)]
    public async Task GetCategoriesForDropdown_ShouldReturnOk_ForValidCategoryType(int categoryType)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={categoryType}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesForDropdown_ShouldReturnOkWithList_ForValidCategoryType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetCategoriesForDropdownResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCategoriesForDropdown_ShouldReturnCategoriesOrderedBySortOrder()
    {
        // Arrange
        const int categoryType = (int)CategoryType.Product;
        string firstName = $"OrderTestCat{Guid.NewGuid():N}";
        string secondName = $"OrderTestCat{Guid.NewGuid():N}";
        string thirdName = $"OrderTestCat{Guid.NewGuid():N}";

        CreateCategoryRequest firstRequest = new(firstName, categoryType);
        CreateCategoryRequest secondRequest = new(secondName, categoryType);
        CreateCategoryRequest thirdRequest = new(thirdName, categoryType);

        using HttpRequestMessage firstHttpRequest = HttpClientExtensions
            .CreatePostMessage("/api/menu/categories")
            .WithJsonContent(firstRequest)
            .AddAuthorizationHeader(UserRoles.Admin);
        using HttpRequestMessage secondHttpRequest = HttpClientExtensions
            .CreatePostMessage("/api/menu/categories")
            .WithJsonContent(secondRequest)
            .AddAuthorizationHeader(UserRoles.Admin);
        using HttpRequestMessage thirdHttpRequest = HttpClientExtensions
            .CreatePostMessage("/api/menu/categories")
            .WithJsonContent(thirdRequest)
            .AddAuthorizationHeader(UserRoles.Admin);

        await apiFactory.HttpClient.SendAsync(firstHttpRequest, TestContext.Current.CancellationToken);
        await apiFactory.HttpClient.SendAsync(secondHttpRequest, TestContext.Current.CancellationToken);
        await apiFactory.HttpClient.SendAsync(thirdHttpRequest, TestContext.Current.CancellationToken);

        // Act
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={categoryType}")
            .AddAuthorizationHeader(UserRoles.Admin);

        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        List<GetCategoriesForDropdownResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<List<GetCategoriesForDropdownResponse>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        List<Category> categoriesFromDb = await apiFactory.MenuDbContext.Categories
            .AsNoTracking()
            .Where(c => c.CategoryType == CategoryType.Product)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(TestContext.Current.CancellationToken);

        List<Guid> expectedOrder = categoriesFromDb.Select(c => c.Id.Value).ToList();
        List<Guid> actualOrder = result.Select(r => r.Id).ToList();

        Assert.Equal(expectedOrder, actualOrder);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    [InlineData(-1)]
    public async Task GetCategoriesForDropdown_ShouldReturnBadRequest_ForInvalidCategoryType(int invalidCategoryType)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={invalidCategoryType}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesForDropdown_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetCategoriesForDropdown_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }
}
