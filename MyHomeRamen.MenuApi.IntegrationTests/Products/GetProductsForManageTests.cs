using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;

using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class GetProductsForManageTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string Endpoint = "/api/menu/products/manage";
    private Product _product = default!;
    private IEnumerable<Product> _products = [];
    private (string KeycloakUserId, Guid UserId) _userId;
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanManageProducts];
    private string _testSuffix = string.Empty;

    public async ValueTask InitializeAsync()
    {
        _testSuffix = Guid.CreateVersion7().ToString("N")[..12];
        _userId = await apiFactory.IdentityTestData.SeedUser(_requiredPermissions, "get-products-manage-user");
        List<Category> productCategories =
        [
            DataGenerator.CreateProductCategory($"Product Category 1 {_testSuffix}", 1),
            DataGenerator.CreateProductCategory($"Product Category 2 {_testSuffix}", 2),
            DataGenerator.CreateProductCategory($"Product Category 3 {_testSuffix}", 3)
        ];
        Category ingredientCategory = DataGenerator.CreateIngredientCategory($"Ingredient Category {_testSuffix}");
        Ingredient ingredient = DataGenerator.CreateIngredient(ingredientCategory, $"Ingredient {_testSuffix}");
        _product = DataGenerator.CreateProduct([ingredient], [], productCategories[0], $"Product 1 {_testSuffix}");
        Product secondProduct = DataGenerator.CreateProduct([ingredient], [], productCategories[1], $"Product 2 {_testSuffix}");
        Product thirdProduct = DataGenerator.CreateProduct([ingredient], [], productCategories[2], $"Product 3 {_testSuffix}");
        _products = [_product, secondProduct, thirdProduct];

        apiFactory.MenuDbContext.Category.AddRange(productCategories);
        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        apiFactory.MenuDbContext.Product.AddRange(_products);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await apiFactory.IdentityTestData.DeleteUser(_userId.UserId);

    [Fact]
    public async Task GetProductsForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
        ProductForManageDto? product = result.Products.SingleOrDefault(p => p.Id == _product.Id.Value);
        Assert.NotNull(product);
        Assert.Equal(_product.Categories[0].Id.Value, product.CategoryId);
        Assert.Equal(_product.BaseIngredients.Select(ingredient => ingredient.Id.Value), product.IngredientIds);
        Assert.Empty(product.CustomIngredientIds);
        Assert.False(result.AllowedActions.CanCreate);
        Assert.False(result.AllowedActions.CanEdit);
        Assert.False(result.AllowedActions.CanRetire);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnIndependentAllowedActions()
    {
        // Arrange
        (string KeycloakUserId, Guid UserId) userId = await apiFactory.IdentityTestData.SeedUser(
            [
                PermissionConstants.CanManageProducts,
                PermissionConstants.CanAddProduct,
                PermissionConstants.CanDeleteProduct
            ],
            "get-products-actions-user");
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(userId);

        try
        {
            // Act
            HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
            GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

            // Assert
            await responseMessage.AssertStatusCode(HttpStatusCode.OK);
            Assert.NotNull(result);
            Assert.True(result.AllowedActions.CanCreate);
            Assert.False(result.AllowedActions.CanEdit);
            Assert.True(result.AllowedActions.CanRetire);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(userId.UserId);
        }
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{Endpoint}?name={Uri.EscapeDataString(partialName)}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.All(result.Products, p => Assert.Contains(partialName, p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnFilteredResults_ByCategoryId()
    {
        // Arrange
        Guid categoryId = _product.Categories[0].Id.Value;

        IEnumerable<Guid> expectedProductIds = _products.Where(p => p.Categories.Any(c => c.Id.Value == categoryId))
                                                        .Select(p => p.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{Endpoint}?categoryIds={categoryId}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

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
        Guid ingredientId = _product.BaseIngredients[0].Id.Value;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{Endpoint}?ingredientIds={ingredientId}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
        Assert.Contains(result.Products, p => p.Id == _product.Id.Value);
    }

    [Fact]
    public async Task GetProductsForManage_ShouldReturnPagedResults()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{Endpoint}?pageSize=1");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsForManageResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.True(result.TotalCount > 1);
        Assert.Single(result.Products);
    }
}
