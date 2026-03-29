using System.Net;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class CreateIngredientTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task CreateIngredient_ShouldReturnCreated_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;
        CreateIngredientRequest request = DataGenerator.GenerateValidIngredient().ToCreateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/ingredients")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
        CreateIngredientRequest request = DataGenerator.GenerateValidIngredient().ToCreateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/ingredients")
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task CreateIngredient_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Forbidden;
        CreateIngredientRequest request = DataGenerator.GenerateValidIngredient().ToCreateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/ingredients")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidCreateIngredientRequests), MemberType = typeof(DataGenerator))]
    public async Task CreateIngredient_ShouldReturnBadRequest_ForInvalidRequest(CreateIngredientRequest request)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/ingredients")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturnBadRequest_ForDuplicateName()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        Domain.Menu.Ingredients.Ingredient existingIngredient = DataGenerator.GeneratedIngredients.First();
        CreateIngredientRequest request = existingIngredient.ToCreateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/ingredients")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }
}
