using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class GetProductByIdForManageTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/menu/products";
    private Product _product = default!;

    public async ValueTask InitializeAsync()
    {
        Category ingredientCategory = DataGenerator.CreateIngredientCategory();
        Category productCategory = DataGenerator.CreateProductCategory();
        Ingredient ingredient = DataGenerator.CreateIngredient(ingredientCategory);
        _product = DataGenerator.CreateProduct([ingredient], [], productCategory);

        apiFactory.MenuDbContext.Category.AddRange([ingredientCategory, productCategory]);
        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}/manage");
        httpRequest.AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(_product.Id.Value, result.Id);
        Assert.Equal(_product.Name, result.Name);
        Assert.Equal(_product.Price, result.Price);
    }

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}/manage");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetProductByIdForManage_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}/manage");
        httpRequest.AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProductByIdForManage_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}/manage");
        httpRequest.AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductByIdForManage_ResponseShouldContainCategoryAndIngredientIds()
    {
        // Arrange
        Guid expectedCategoryId = _product.Categories[0].Id.Value;
        IEnumerable<Guid> expectedIngredientIds = _product.BaseIngredients.Select(i => i.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}/manage");
        httpRequest.AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductByIdForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(expectedCategoryId, result.CategoryId);
        Assert.Equal(expectedIngredientIds.OrderBy(id => id), result.IngredientIds.OrderBy(id => id));
    }
}
