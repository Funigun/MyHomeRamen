using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.MenuApi.IntegrationTests.Ingredients;

public sealed class GetIngredientByIdTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/menu/ingredients";
    private Ingredient _ingredient = default!;

    public async ValueTask InitializeAsync()
    {
        _ingredient = DataGenerator.GeneratedIngredients.First();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetIngredientById_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{_ingredient.Id.Value}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(_ingredient.Id.Value, result.Id);
        Assert.Equal(_ingredient.Name, result.Name);
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{_ingredient.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetIngredientById_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{_ingredient.Id.Value}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetIngredientById_ResponseShouldContainCategoryIds()
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First(i => i.Categories.Count > 0);
        IEnumerable<Guid> expectedIds = ingredient.Categories.Select(c => c.Id.Value);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{ingredient.Id.Value}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(expectedIds.OrderBy(id => id), result.CategoryIds.OrderBy(id => id));
    }
}
