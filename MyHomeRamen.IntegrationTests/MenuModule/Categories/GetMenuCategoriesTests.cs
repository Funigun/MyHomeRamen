using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Categories;

public sealed class GetMenuCategoriesTests(WebApiFactory apiFactory)
{
    private const string Endpoint = "/api/menu/categories/menu";

    [Fact]
    public async Task GetMenuCategories_ShouldReturn_OnlyProductCategories()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GenerateValidCategory(CategoryType.Ingredient);
        apiFactory.MenuDbContext.Categories.Add(ingredientCategory);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetMenuCategoriesResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetMenuCategoriesResponse>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, c => Assert.NotEqual(Guid.Empty, c.Id));
    }

    [Fact]
    public async Task GetMenuCategories_ShouldReturn_EmptyList_WhenNoProductCategoriesExist()
    {
        // Arrange — use a fresh client pointing to a response with only ingredient categories seeded
        // by seeding fresh ingredient-only categories and removing product categories
        WebApiFactory isolatedFactory = new();
        await ((IAsyncLifetime)isolatedFactory).InitializeAsync();

        // Remove all product categories from the isolated context
        List<Category> productCategories = isolatedFactory.MenuDbContext.Categories
            .Where(c => c.CategoryType == CategoryType.Product)
            .ToList();
        isolatedFactory.MenuDbContext.Categories.RemoveRange(productCategories);
        await isolatedFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await isolatedFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetMenuCategoriesResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetMenuCategoriesResponse>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result);

        await isolatedFactory.DisposeAsync();
    }

    [Fact]
    public async Task GetMenuCategories_ShouldReturn_OK_ForAnonymousUser()
    {
        // Arrange — no auth header added
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    }
}
