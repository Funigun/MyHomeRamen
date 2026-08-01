using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class GetCategoriesByTypeTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/menu/categories/by-type";

    

    public async ValueTask InitializeAsync()
    {
        Category productCategory = DataGenerator.CreateProductCategory();
        Category secondProductCategory = DataGenerator.CreateProductCategory();
        Category prodcutCategoryDuplicateCheck = DataGenerator.CreateProductCategory();
        IEnumerable<Category> ingredientCategories = DataGenerator.CreateIngredientCategories();

        apiFactory.MenuDbContext.Category.AddRange(new[] { productCategory, secondProductCategory, prodcutCategoryDuplicateCheck }.Concat(ingredientCategories));
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOkWithList_ForIngredientType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Ingredient}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetCategoriesByTypeResponse>? result = await responseMessage.Content.ReadFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnOk_ForAuthenticatedManager()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetCategoriesByTypeResponse>? result = await responseMessage.Content.ReadFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType={(int)CategoryType.Product}")
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnBadRequest_ForInvalidCategoryType()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}?categoryType=999")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
