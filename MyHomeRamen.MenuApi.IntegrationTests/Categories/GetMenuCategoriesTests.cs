using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class GetMenuCategoriesTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    private const string Endpoint = "/api/menu/categories/menu";

    [Fact]
    public async Task GetMenuCategories_ShouldReturn_OnlyProductCategories()
    {
        // Arrange
        Category productCategory = DataGenerator.CreateProductCategory();
        Category ingredientCategory = DataGenerator.CreateIngredientCategory();
        apiFactory.MenuDbContext.Category.AddRange([ingredientCategory, productCategory]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetMenuCategoriesResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetMenuCategoriesResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Categories);
        Assert.All(result.Categories, c => Assert.NotEqual(Guid.Empty, c.Id));
        Assert.All(result.Categories, c => Assert.NotEqual(ingredientCategory.Id.Value, c.Id));
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
