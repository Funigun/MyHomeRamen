using System.Net;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class CreateCategoryTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task CreateCategory_ShouldReturnCreated_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;
        CreateCategoryRequest request = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        string mess = await responseMessage.Content.ReadAsStringAsync();
        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
        CreateCategoryRequest request = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task CreateCategory_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Forbidden;
        CreateCategoryRequest request = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidCreateCategoryRequests), MemberType = typeof(DataGenerator))]
    public async Task CreateCategory_ShouldReturnBadRequest_ForInvalidRequest(CreateCategoryRequest request)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnBadRequest_ForDuplicateName()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        Category existingCategory = DataGenerator.GeneratedCategories.First();
        CreateCategoryRequest request = existingCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(responseMessage.StatusCode == expectedStatusCode, $"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
    }

    [Fact]
    public async Task CreateCategory_ShouldAssignSequentialSortOrder_ForCategoryType()
    {
        // Arrange
        const int categoryType = (int)CategoryType.Product;
        string firstName = $"SeqCat{Guid.NewGuid():N}";
        string secondName = $"SeqCat{Guid.NewGuid():N}";

        CreateCategoryRequest firstRequest = new(firstName, categoryType);
        CreateCategoryRequest secondRequest = new(secondName, categoryType);

        using HttpRequestMessage firstHttpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                        .WithJsonContent(firstRequest)
                                                                        .AddAuthorizationHeader(UserRoles.Admin);
        using HttpRequestMessage secondHttpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                         .WithJsonContent(secondRequest)
                                                                         .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage firstResponse = await apiFactory.HttpClient.SendAsync(firstHttpRequest, TestContext.Current.CancellationToken);
        HttpResponseMessage secondResponse = await apiFactory.HttpClient.SendAsync(secondHttpRequest, TestContext.Current.CancellationToken);

        CreateCategoryResponse firstResult = await firstResponse.ResponseToDto<CreateCategoryResponse>();
        CreateCategoryResponse secondResult = await secondResponse.ResponseToDto<CreateCategoryResponse>();

        Category? firstCategory = await apiFactory.MenuDbContext.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == (CategoryId)firstResult.Id, TestContext.Current.CancellationToken);
        Category? secondCategory = await apiFactory.MenuDbContext.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == (CategoryId)secondResult.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(firstCategory);
        Assert.NotNull(secondCategory);
        Assert.Equal(firstCategory.SortOrder + 1, secondCategory.SortOrder);
    }
}
