using System.Net;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Ingredients;

public sealed class DeleteIngredientTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteIngredient_ShouldReturnNoContent_ForValidId()
    {
        // Arrange — seed a standalone ingredient not referenced by any product
        Category ingredientCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Ingredient);

        Ingredient standalone = Ingredient.Create(
            Guid.NewGuid(),
            $"DelTest_{Guid.NewGuid():N}",
            "Standalone test ingredient",
            0.50m,
            [ingredientCategory]);

        apiFactory.MenuDbContext.Ingredient.Add(standalone);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateDeleteMessage($"/api/menu/ingredients/{standalone.Id}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 204 returned
        await response.AssertStatusCode(HttpStatusCode.NoContent);

        // Assert — deleted record no longer exists in DB
        bool stillExists = await apiFactory.MenuDbContext.Ingredient.Exists(i => i.Id == standalone.Id, TestContext.Current.CancellationToken);
        Assert.False(stillExists, "Deleted ingredient should no longer exist in DB.");
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/menu/ingredients/{id}");

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
        Guid id = DataGenerator.GeneratedIngredients.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateDeleteMessage($"/api/menu/ingredients/{id}")
            .AddAuthorizationHeader(role);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateDeleteMessage($"/api/menu/ingredients/{nonExistentId}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsBaseIngredient()
    {
        // Arrange — derive ingredient from a tracked generated product so the reference is guaranteed
        Ingredient usedIngredient = DataGenerator.GeneratedProducts.First().BaseIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateDeleteMessage($"/api/menu/ingredients/{usedIngredient.Id}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsCustomIngredient()
    {
        // Arrange — derive ingredient from a tracked generated product so the reference is guaranteed
        Ingredient usedIngredient = DataGenerator.GeneratedProducts.First().CustomIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateDeleteMessage($"/api/menu/ingredients/{usedIngredient.Id}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
