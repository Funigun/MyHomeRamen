using System.Net;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class CreateProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Product _product = default!;

    public async ValueTask InitializeAsync()
    {
        _product = DataGenerator.GenerateValidProduct();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task CreateProduct_ShouldReturnLocationHeader_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnNotAuthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidCreateProductRequests), MemberType = typeof(DataGenerator))]
    public async Task CreateProduct_ShouldReturnBadRequest_ForInvalidRequest(CreateProductRequest request)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }
}
