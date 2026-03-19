using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class CreateProductTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task CreateProduct_ShouldReturnLocationHeader_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;

        CreateProductRequest request = DataGenerator.GenerateValidProduct().ToCreateProductRequest();

        apiFactory.HttpClient.AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.PostAsJsonAsync("/api/menu/products", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnNotAuthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

        CreateProductRequest request = DataGenerator.GenerateValidProduct().ToCreateProductRequest();

        // Act
        apiFactory.HttpClient.ClearAuthorizationHeaders();
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.PostAsJsonAsync("/api/menu/products", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task CreateProduct_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Forbidden;

        CreateProductRequest request = DataGenerator.GenerateValidProduct().ToCreateProductRequest();

        apiFactory.HttpClient.AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.PostAsJsonAsync("/api/menu/products", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidCreateProductRequests), MemberType = typeof(DataGenerator))]
    public async Task CreateProduct_ShouldReturnBadRequest_ForInvalidRequest(CreateProductRequest request)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

        apiFactory.HttpClient.AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.PostAsJsonAsync("/api/menu/products", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }
}
