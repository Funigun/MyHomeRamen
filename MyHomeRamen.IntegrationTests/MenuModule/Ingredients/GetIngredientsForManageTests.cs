using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Ingredients;

public sealed class GetIngredientsForManageTests(WebApiFactory apiFactory)
{
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
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
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
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
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
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldFilterByName_WhenNameProvided()
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();
        string partialName = ingredient.Name[..5];

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/ingredients/manage?name={Uri.EscapeDataString(partialName)}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForManageResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.All(result.Ingredients, i => Assert.Contains(partialName, i.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetIngredientsForManage_ShouldFilterByCategories_WhenCategoryIdsProvided()
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();
        Guid categoryId = ingredient.Categories.First().Id.Value;

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
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
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
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Ingredients);
    }

    }
