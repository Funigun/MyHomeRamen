using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.MenuApi.IntegrationTests.Ingredients;

public sealed class GetIngredientsForManageTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Ingredient _ingredient = default!;

    public async ValueTask InitializeAsync()
    {
        _ingredient = DataGenerator.GeneratedIngredients.First();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetIngredientsForManage_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage("/api/menu/ingredients/manage")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Ingredients);
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage("/api/menu/ingredients/manage");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetIngredientsForManage_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage("/api/menu/ingredients/manage")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldFilterByName_WhenNameProvided()
    {
        // Arrange
        string partialName = _ingredient.Name[..5];

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/ingredients/manage?name={Uri.EscapeDataString(partialName)}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.All(result.Ingredients, i => Assert.Contains(partialName, i.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldFilterByCategories_WhenCategoryIdsProvided()
    {
        // Arrange
        Guid categoryId = _ingredient.Categories.First().Id.Value;

        IEnumerable<Guid> expectedIngredientIds = DataGenerator.GeneratedIngredients
            .Where(i => i.Categories.Any(c => c.Id.Value == categoryId))
            .Select(i => i.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/ingredients/manage?categoryIds={categoryId}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Ingredients);
        Assert.All(result.Ingredients, i => Assert.Contains(i.Id, expectedIngredientIds));
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldReturnEmptyList_WhenNoIngredientsMatchFilters()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/ingredients/manage?name={Guid.NewGuid()}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Empty(result.Ingredients);
    }
}
