using System.Net;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class DeleteProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private (string KeycloakUserId, Guid UserId) _userId;
    private Category _productCategory = default!;
    private Category _ingredientCategory = default!;
    private Ingredient _ingredient = default!;
    private Product _product = default!;

    public async ValueTask InitializeAsync()
    {
        _userId = await apiFactory.IdentityTestData.SeedUser(
            [PermissionConstants.CanManageProducts, PermissionConstants.CanDeleteProduct],
            "delete-product-user");
        _productCategory = DataGenerator.CreateProductCategory();
        _ingredientCategory = DataGenerator.CreateIngredientCategory();
        _ingredient = DataGenerator.CreateIngredient(_ingredientCategory);
        _product = DataGenerator.CreateProduct([_ingredient], [], _productCategory);

        apiFactory.MenuDbContext.Category.AddRange([_productCategory, _ingredientCategory]);
        apiFactory.MenuDbContext.Ingredient.Add(_ingredient);
        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await apiFactory.IdentityTestData.DeleteUser(_userId.UserId);
        Product? remainingProduct = await apiFactory.MenuDbContext.Product.Query().ById(
            _product.Id,
            TestContext.Current.CancellationToken);
        if (remainingProduct is not null)
        {
            apiFactory.MenuDbContext.Product.Delete(remainingProduct);
        }

        apiFactory.MenuDbContext.Ingredient.Delete(_ingredient);
        apiFactory.MenuDbContext.Category.Delete(_productCategory);
        apiFactory.MenuDbContext.Category.Delete(_ingredientCategory);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_ForAuthorizedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/products/{_product.Id.Value}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.NoContent);
        Assert.False(await apiFactory.MenuDbContext.Product.Exists(
            product => product.Id == _product.Id,
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        using HttpRequestMessage httpRequest =
            HttpClientExtensions.CreateDeleteMessage($"/api/menu/products/{Guid.NewGuid()}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnForbidden_WhenUserLacksDeletePermission()
    {
        await AssertForbiddenForPermissionsAsync(
            [PermissionConstants.CanManageProducts],
            "delete-product-without-delete");
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnForbidden_WhenUserLacksManagePermission()
    {
        await AssertForbiddenForPermissionsAsync(
            [PermissionConstants.CanDeleteProduct],
            "delete-product-without-manage");
    }

    private async Task AssertForbiddenForPermissionsAsync(IEnumerable<string> permissions, string userName)
    {
        // Arrange
        (string KeycloakUserId, Guid UserId) userId =
            await apiFactory.IdentityTestData.SeedUser(permissions, userName);
        using HttpRequestMessage httpRequest =
            HttpClientExtensions.CreateDeleteMessage($"/api/menu/products/{_product.Id.Value}");
        httpRequest.AddAuthorizationHeader(userId);

        try
        {
            // Act
            HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

            // Assert
            await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(userId.UserId);
        }
    }
}
