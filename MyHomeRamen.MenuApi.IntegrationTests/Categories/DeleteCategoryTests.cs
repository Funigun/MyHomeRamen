using System.Net;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
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

    public async ValueTask InitializeAsync()
    {
        _ingredientcategory = DataGenerator.CreateIngredientCategory();
        _productCategory = DataGenerator.CreateProductCategory();
        _ingredient = DataGenerator.CreateIngredient(_ingredientcategory);
        _product = DataGenerator.CreateProduct([_ingredient], [], _productCategory);

        apiFactory.MenuDbContext.Category.AddRange([_ingredientcategory, _productCategory]);
        apiFactory.MenuDbContext.Ingredient.Add(_ingredient);
        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_ForValidId()
    {
        // Arrange
        const CategoryType categoryType = CategoryType.Product;
        int numOfExistingCategories = await apiFactory.MenuDbContext.Category.Count(TestContext.Current.CancellationToken);

        Category cat1 = Category.Create(Guid.NewGuid(), $"DelTest1_{Guid.NewGuid():N}", numOfExistingCategories + 1, categoryType);
        Category cat2 = Category.Create(Guid.NewGuid(), $"DelTest2_{Guid.NewGuid():N}", numOfExistingCategories + 2, categoryType);
        Category cat3 = Category.Create(Guid.NewGuid(), $"DelTest3_{Guid.NewGuid():N}", numOfExistingCategories + 3, categoryType);

        apiFactory.MenuDbContext.Category.AddRange([cat1, cat2, cat3]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Guid idToDelete = cat2.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{idToDelete}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 204 returned
        await response.AssertStatusCode(HttpStatusCode.NoContent);

        // Assert — deleted record no longer exists in DB
        bool stillExists = await apiFactory.MenuDbContext.Category.Exists(c => c.Id == new CategoryId(idToDelete), TestContext.Current.CancellationToken);
        Assert.False(stillExists, "Deleted category should no longer exist in DB.");

        // Assert — ALL remaining categories of the same type have contiguous sort orders starting from 1
        IEnumerable<Category> allRemaining = await apiFactory.MenuDbContext.Category.Query().GetByIds([cat1.Id, cat3.Id], TestContext.Current.CancellationToken);

        for (int i = 0; i < allRemaining.Count(); i++)
        {
            Assert.Equal(i + 1, allRemaining.ElementAt(i).SortOrder);
        }

        // Assert — cat1 and cat3 are adjacent with cat3 immediately following cat1
        int cat1Index = allRemaining.ToList().FindIndex(c => c.Id == cat1.Id);
        int cat3Index = allRemaining.ToList().FindIndex(c => c.Id == cat3.Id);
        Assert.Equal(cat1Index + 1, cat3Index);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{nonExistentId}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryIsUsedByProduct()
    {
        // Arrange
           using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_productCategory.Id.Value}")
                                                                      .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryIsUsedByIngredient()
    {
        // Arrange — derive category from a tracked generated ingredient so the reference is guaranteed
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_ingredientcategory.Id.Value}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{_productCategory.Id.Value}")
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_ForEmptyGuid()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{Guid.Empty}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
