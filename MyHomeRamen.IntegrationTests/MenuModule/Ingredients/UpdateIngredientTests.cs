using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Ingredients;

public sealed class UpdateIngredientTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task UpdateIngredient_ShouldReturnOk_ForValidRequest()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Ingredient);

        Ingredient ingredient = Ingredient.Create(
            Guid.NewGuid(),
            $"UpdateTest_{Guid.NewGuid():N}",
            "Original description text",
            1.50m,
            [ingredientCategory]);

        apiFactory.MenuDbContext.Ingredients.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateIngredientRequest request = new(
            $"Updated_{Guid.NewGuid():N}",
            "Updated description text here",
            3.99m,
            [ingredientCategory.Id.Value]);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredient.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        string responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert — 200 returned
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode} with `{responseContent}`.");

        UpdateIngredientResponse? result = await response.Content.ReadFromJsonAsync<UpdateIngredientResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(ingredient.Id.Value, result.Id);

        // Assert — updated fields persisted
        Ingredient? updated = await apiFactory.MenuDbContext.Ingredients
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(updated);
        Assert.Equal(request.Name, updated.Name);
        Assert.Equal(request.Description, updated.Description);
        Assert.Equal(request.Price, updated.Price);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(DataGenerator.GeneratedIngredients.First().ToUpdateIngredientRequest());

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"Expected 401 Unauthorized but got {response.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task UpdateIngredient_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredient.Id}")
            .WithJsonContent(ingredient.ToUpdateIngredientRequest())
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"Expected 403 Forbidden but got {response.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{nonExistentId}")
            .WithJsonContent(ingredient.ToUpdateIngredientRequest())
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateIngredientRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForInvalidRequest(UpdateIngredientRequest request)
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_WhenNameAlreadyTakenByDifferentIngredient()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Ingredient);

        Ingredient ingredientA = Ingredient.Create(
            Guid.NewGuid(),
            $"IngredientA_{Guid.NewGuid():N}",
            "Description for ingredient A",
            1.00m,
            [ingredientCategory]);

        Ingredient ingredientB = Ingredient.Create(
            Guid.NewGuid(),
            $"IngredientB_{Guid.NewGuid():N}",
            "Description for ingredient B",
            2.00m,
            [ingredientCategory]);

        apiFactory.MenuDbContext.Ingredients.AddRange(ingredientA, ingredientB);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateIngredientRequest request = ingredientA.ToUpdateIngredientRequest() with { Name = ingredientB.Name };

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredientA.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnOk_WhenNameIsUnchanged()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Ingredient);

        Ingredient ingredient = Ingredient.Create(
            Guid.NewGuid(),
            $"SameNameTest_{Guid.NewGuid():N}",
            "Description stays the same here",
            2.50m,
            [ingredientCategory]);

        apiFactory.MenuDbContext.Ingredients.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateIngredientRequest request = ingredient.ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredient.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — self-rename passes name-uniqueness check
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected 200 OK but got {response.StatusCode}.");
    }
}
