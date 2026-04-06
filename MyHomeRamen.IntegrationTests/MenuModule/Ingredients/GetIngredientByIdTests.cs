using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Ingredients;

public sealed class GetIngredientByIdTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/menu/ingredients";

    [Fact]
    public async Task GetIngredientById_ShouldReturnOk_ForAuthenticatedAdmin()
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{ingredient.Id.Value}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetIngredientByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(ingredient.Id.Value, result.Id);
        Assert.Equal(ingredient.Name, result.Name);
        Assert.Equal(ingredient.Description, result.Description);
        Assert.Equal(ingredient.Price, result.Price);
        Assert.NotEmpty(result.Categories);
        Assert.All(result.Categories, c => Assert.Contains(ingredient.Categories, ic => ic.Id.Value == c.Id && ic.Name == c.Name));
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{ingredient.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetIngredientById_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{ingredient.Id.Value}")
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnNotFound_ForNonExistentIngredient()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
    }
}
