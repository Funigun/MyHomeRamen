using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
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
        apiFactory.MenuDbContext.Category.Add(ingredientCategory);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetMenuCategoriesResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetMenuCategoriesResponse>>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, c => Assert.NotEqual(Guid.Empty, c.Id));
    }

    [Fact]
    public async Task GetMenuCategories_ShouldReturn_OK_ForAnonymousUser()
    {
        // Arrange — no auth header added
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
    }
}
