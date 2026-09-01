using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class DeleteCategoryTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Category _productCategory = default!;
    private Category _ingredientcategory = default!;
    private Ingredient _ingredient = default!;
    private Product _product = default!;
    private (string KeycloakUserId, Guid UserId) _userId;
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanDeleteCategory];

    public async ValueTask InitializeAsync()
    {
        _userId = await apiFactory.IdentityTestData.SeedUser(_requiredPermissions, "delete-category-user");
        _ingredientcategory = DataGenerator.CreateIngredientCategory();
        _productCategory = DataGenerator.CreateProductCategory();
        _ingredient = DataGenerator.CreateIngredient(_ingredientcategory);
        _product = DataGenerator.CreateProduct([_ingredient], [], _productCategory);

        apiFactory.MenuDbContext.Category.AddRange([_ingredientcategory, _productCategory]);
        apiFactory.MenuDbContext.Ingredient.Add(_ingredient);
        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await apiFactory.IdentityTestData.DeleteUser(_userId.UserId);
        apiFactory.MenuDbContext.Product.Delete(_product);
        apiFactory.MenuDbContext.Ingredient.Delete(_ingredient);
        apiFactory.MenuDbContext.Category.Delete(_ingredientcategory);
        apiFactory.MenuDbContext.Category.Delete(_productCategory);

        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_ForValidId()
    {
        // Arrange
        const CategoryType categoryType = CategoryType.Product;
        int numOfExistingCategories = 1;

        Category cat1 = Category.Create(Guid.NewGuid(), $"DelTest1_{Guid.NewGuid():N}", numOfExistingCategories + 1, categoryType);
        Category cat2 = Category.Create(Guid.NewGuid(), $"DelTest2_{Guid.NewGuid():N}", numOfExistingCategories + 2, categoryType);
        Category cat3 = Category.Create(Guid.NewGuid(), $"DelTest3_{Guid.NewGuid():N}", numOfExistingCategories + 3, categoryType);

        apiFactory.MenuDbContext.Category.AddRange([cat1, cat2, cat3]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Guid idToDelete = cat2.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{idToDelete}");
        httpRequest.AddAuthorizationHeader(_userId);
        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 204 returned
        await response.AssertStatusCode(HttpStatusCode.NoContent);

        // Assert — deleted record no longer exists in DB
        bool stillExists = await apiFactory.MenuDbContext.Category.Exists(c => c.Id == new CategoryId(idToDelete), TestContext.Current.CancellationToken);
        Assert.False(stillExists, "Deleted category should no longer exist in DB.");

        // Assert — ALL remaining categories of the same type have contiguous sort orders starting from 1
        using HttpRequestMessage assertRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/categories/by-type?categoryType={(int)CategoryType.Product}");
        assertRequest.AddAuthorizationHeader(_userId);

        HttpResponseMessage assertResponse = await apiFactory.HttpClient.SendAsync(assertRequest, TestContext.Current.CancellationToken);

        GetCategoriesByTypeResponse productCategories = (await assertResponse.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken))!;
        IEnumerable<CategoryByTypeDto> allRemaining = productCategories.Categories.OrderBy(c => c.SortOrder);

        for (int i = 0; i < allRemaining.Count(); i++)
        {
            Assert.Equal(i + 1, allRemaining.ElementAt(i).SortOrder);
        }

        // Assert — cat1 and cat3 are adjacent with cat3 immediately following cat1
        int cat1Index = allRemaining.ToList().FindIndex(c => c.Id == cat1.Id.Value);
        int cat3Index = allRemaining.ToList().FindIndex(c => c.Id == cat3.Id.Value);
        Assert.Equal(cat1Index + 1, cat3Index);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{nonExistentId}");
        httpRequest.AddAuthorizationHeader(_userId);
        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryIsUsedByProduct()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_productCategory.Id.Value}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryIsUsedByIngredient()
    {
        // Arrange — derive category from a tracked generated ingredient so the reference is guaranteed
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_ingredientcategory.Id.Value}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_productCategory.Id.Value}");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task DeleteCategory_ShouldReturnForbidden_ForNonManagerRoles(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_productCategory.Id.Value}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));
        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_ForEmptyGuid()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{Guid.Empty}");
        httpRequest.AddAuthorizationHeader(_userId);
        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
