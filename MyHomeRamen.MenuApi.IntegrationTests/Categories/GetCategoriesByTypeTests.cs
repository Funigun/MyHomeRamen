using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class GetCategoriesByTypeTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/menu/categories/by-type";
    private IEnumerable<Category> _categories = [];

    public async ValueTask InitializeAsync()
    {
        Category productCategory = DataGenerator.CreateProductCategory();
        Category secondProductCategory = DataGenerator.CreateProductCategory();
        Category prodcutCategoryDuplicateCheck = DataGenerator.CreateProductCategory();
        IEnumerable<Category> ingredientCategories = DataGenerator.CreateIngredientCategories();

        _categories = new[] { productCategory, secondProductCategory, prodcutCategoryDuplicateCheck }.Concat(ingredientCategories);

        apiFactory.MenuDbContext.Category.AddRange(_categories);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (Category category in _categories)
        {
            apiFactory.MenuDbContext.Category.Delete(category);
        }

        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOkWithList_ForIngredientType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Ingredient}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesByTypeResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Categories);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOk_ForAuthenticatedManager()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesByTypeResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Categories);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetCategoriesByType_ShouldReturnForbidden_ForNonManagerRoles(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnBadRequest_ForInvalidCategoryType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType=999");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
