using System.Net;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Ingredients;

public sealed class DeleteIngredientTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Ingredient _standaloneIngredient = default!;
    private Ingredient _productIngredient = default!;
    private Ingredient _customIngredient = default!;
    private Category _ingredientCategory = default!;
    private (string KeycloakUserId, Guid UserId) _userId;
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanManageIngredients, PermissionConstants.CanDeleteIngredient];

    public async ValueTask InitializeAsync()
    {
        _userId = await apiFactory.IdentityTestData.SeedUser(_requiredPermissions, "delete-ingredient-user");
        _ingredientCategory = DataGenerator.CreateIngredientCategory();
        _standaloneIngredient = DataGenerator.CreateIngredient(_ingredientCategory);
        _productIngredient = DataGenerator.CreateIngredient(_ingredientCategory);
        _customIngredient = DataGenerator.CreateIngredient(_ingredientCategory);

        Category productCategory = DataGenerator.CreateProductCategory();
        Product product = DataGenerator.CreateProduct([_productIngredient], [], productCategory);
        Product productWithCustoms = DataGenerator.CreateProduct([_productIngredient], [_customIngredient], productCategory);
        apiFactory.MenuDbContext.Ingredient.Add(_standaloneIngredient);
        apiFactory.MenuDbContext.Product.AddRange([product, productWithCustoms]);

        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await apiFactory.IdentityTestData.DeleteUser(_userId.UserId);

    [Fact]
    public async Task DeleteIngredient_ShouldReturnNoContent_ForValidId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{_standaloneIngredient.Id}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 204 returned
        await response.AssertStatusCode(HttpStatusCode.NoContent);

        // Assert — deleted record no longer exists in DB
        bool stillExists = await apiFactory.MenuDbContext.Ingredient.Exists(i => i.Id == _standaloneIngredient.Id, TestContext.Current.CancellationToken);
        Assert.False(stillExists, "Deleted ingredient should no longer exist in DB.");
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{_standaloneIngredient.Id.Value}");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task DeleteIngredient_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{_standaloneIngredient.Id.Value}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{nonExistentId}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsBaseIngredient()
    {
        // Arrange — derive ingredient from a tracked generated product so the reference is guaranteed
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{_productIngredient.Id.Value}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsCustomIngredient()
    {
        // Arrange — derive ingredient from a tracked generated product so the reference is guaranteed
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{_customIngredient.Id.Value}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
