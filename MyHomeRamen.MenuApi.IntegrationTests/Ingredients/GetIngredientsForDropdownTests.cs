using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Ingredients;

public sealed class GetIngredientsForDropdownTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        Category category = DataGenerator.CreateIngredientCategory();
        Ingredient ingredient = DataGenerator.CreateIngredient(category);

        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetIngredientsForDropdown_ShouldReturnOkWithList_ForAuthenticatedManager()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage("/api/menu/ingredients/dropdown");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetIngredientsForDropdownResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetIngredientsForDropdownResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Ingredients);
    }

    [Fact]
    public async Task GetIngredientsForDropdown_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage("/api/menu/ingredients/dropdown");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task GetIngredientsForDropdown_ShouldReturnForbidden_ForNonManagerRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage("/api/menu/ingredients/dropdown");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }
}
