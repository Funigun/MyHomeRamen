using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
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
        Ingredient ingredient = DataGenerator.GenerateValidIngredient();
        apiFactory.MenuDbContext.Ingredients.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Guid id = ingredient.Id.Value;
        UpdateIngredientRequest request = ingredient.ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.OK,
            $"Expected 200 OK but got {responseMessage.StatusCode}.");

        UpdateIngredientResponse? result = await responseMessage.Content.ReadFromJsonAsync<UpdateIngredientResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);

        Ingredient? updated = await apiFactory.MenuDbContext.Ingredients
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == (IngredientId)id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);
        Assert.Equal(request.Name, updated.Name);
        Assert.Equal(request.Description, updated.Description);
        Assert.Equal(request.Price, updated.Price);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id.Value;
        UpdateIngredientRequest request = DataGenerator.GeneratedIngredients.First().ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 401 Unauthorized but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task UpdateIngredient_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id.Value;
        UpdateIngredientRequest request = DataGenerator.GeneratedIngredients.First().ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 403 Forbidden but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        UpdateIngredientRequest request = DataGenerator.GeneratedIngredients.First().ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{nonExistentId}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400 BadRequest but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateIngredientRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForInvalidRequest(UpdateIngredientRequest request)
    {
        // Arrange
        Guid id = DataGenerator.GeneratedIngredients.First().Id.Value;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400 BadRequest but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_WhenNameAlreadyTakenByDifferentIngredient()
    {
        // Arrange — seed two fresh ingredients to avoid name collision with seeded data
        Ingredient ingredientA = DataGenerator.GenerateValidIngredient();
        Ingredient ingredientB = DataGenerator.GenerateValidIngredient();
        apiFactory.MenuDbContext.Ingredients.AddRange(ingredientA, ingredientB);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Try to update A with B's name
        UpdateIngredientRequest request = new(
            ingredientB.Name,
            ingredientA.Description,
            ingredientA.Price,
            ingredientA.Categories.Select(c => (Guid)c.Id));

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredientA.Id.Value}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400 BadRequest but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnOk_WhenNameIsUnchanged()
    {
        // Arrange — seed a fresh ingredient
        Ingredient ingredient = DataGenerator.GenerateValidIngredient();
        apiFactory.MenuDbContext.Ingredients.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Use the same name (self-rename should pass name-uniqueness check)
        UpdateIngredientRequest request = ingredient.ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredient.Id.Value}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == HttpStatusCode.OK,
            $"Expected 200 OK but got {responseMessage.StatusCode}.");
    }
}
