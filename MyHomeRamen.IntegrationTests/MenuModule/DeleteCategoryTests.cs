using System.Net;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class DeleteCategoryTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_ForValidId()
    {
        // Arrange — 3 new categories of Product type with high sort orders to avoid collision with seeded data
        const CategoryType categoryType = CategoryType.Product;
        Category cat1 = Category.Create(Guid.NewGuid(), $"DelTest1_{Guid.NewGuid():N}", 900, categoryType);
        Category cat2 = Category.Create(Guid.NewGuid(), $"DelTest2_{Guid.NewGuid():N}", 901, categoryType);
        Category cat3 = Category.Create(Guid.NewGuid(), $"DelTest3_{Guid.NewGuid():N}", 902, categoryType);

        apiFactory.MenuDbContext.Categories.AddRange(cat1, cat2, cat3);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Guid idToDelete = cat2.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{idToDelete}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 204 returned
        Assert.True(response.StatusCode == HttpStatusCode.NoContent,
            $"Expected 204 NoContent but got {response.StatusCode}.");

        // Assert — deleted record no longer exists in DB
        bool stillExists = await apiFactory.MenuDbContext.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == (CategoryId)idToDelete, TestContext.Current.CancellationToken);
        Assert.False(stillExists, "Deleted category should no longer exist in DB.");

        // Assert — ALL remaining categories of the same type have contiguous sort orders starting from 1
        List<Category> allRemaining = await apiFactory.MenuDbContext.Categories
            .AsNoTracking()
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(TestContext.Current.CancellationToken);

        for (int i = 0; i < allRemaining.Count; i++)
        {
            Assert.Equal(i + 1, allRemaining[i].SortOrder);
        }

        // Assert — cat1 and cat3 are adjacent with cat3 immediately following cat1
        int cat1Index = allRemaining.FindIndex(c => c.Id == (CategoryId)cat1.Id);
        int cat3Index = allRemaining.FindIndex(c => c.Id == (CategoryId)cat3.Id);
        Assert.Equal(cat1Index + 1, cat3Index);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNotFound_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{nonExistentId}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.NotFound,
            $"Expected 404 NotFound but got {response.StatusCode}.");
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnConflict_WhenCategoryIsUsedByProduct()
    {
        // Arrange — derive category from a tracked generated product so the reference is guaranteed
        Category usedCategory = DataGenerator.GeneratedProducts.First().Categories.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{usedCategory.Id}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Conflict,
            $"Expected 409 Conflict but got {response.StatusCode}.");
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnConflict_WhenCategoryIsUsedByIngredient()
    {
        // Arrange — derive category from a tracked generated ingredient so the reference is guaranteed
        Category usedCategory = DataGenerator.GeneratedIngredients.First().Categories.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{usedCategory.Id}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Conflict,
            $"Expected 409 Conflict but got {response.StatusCode}.");
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = DataGenerator.GeneratedCategories.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{id}");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 401 Unauthorized but got {response.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task DeleteCategory_ShouldReturnForbidden_ForNonManagerRoles(UserRoles role)
    {
        // Arrange
        Guid id = DataGenerator.GeneratedCategories.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/categories/{id}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 403 Forbidden but got {response.StatusCode}.");
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
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400 BadRequest but got {response.StatusCode}.");
    }
}
