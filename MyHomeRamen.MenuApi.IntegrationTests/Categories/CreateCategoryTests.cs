using System.Net;
using Bogus;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class CreateCategoryTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Category _productCategory = default!;

    public async ValueTask InitializeAsync()
    {
        _productCategory = DataGenerator.CreateProductCategory();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task CreateCategory_ShouldReturnCreated_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;
        CreateCategoryRequest request = _productCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
        CreateCategoryRequest request = _productCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task CreateCategory_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Forbidden;
        CreateCategoryRequest request = _productCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidCreateCategoryRequests), MemberType = typeof(CreateCategoryTests))]
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
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    public static TheoryData<CreateCategoryRequest> InvalidCreateCategoryRequests()
    {
        Faker faker = new();
        const int validCategoryType = (int)CategoryType.Product;

        return
        [
            // Name: empty
            new CreateCategoryRequest(string.Empty, validCategoryType),

            // Name: too short
            new CreateCategoryRequest(faker.Random.String2(1, CategoryNameValidator.MinLength - 1), validCategoryType),

            // Name: too long
            new CreateCategoryRequest(faker.Random.String2(CategoryNameValidator.MaxLength + 1, CategoryNameValidator.MaxLength + 10), validCategoryType),

            // CategoryType: invalid
            new CreateCategoryRequest(faker.Random.String2(CategoryNameValidator.MinLength, CategoryNameValidator.MaxLength), 999),
        ];
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnBadRequest_ForDuplicateName()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        CreateCategoryRequest request = _productCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    [Fact]
    public async Task CreateCategory_ShouldAssignSequentialSortOrder_ForCategoryType()
    {
        // Arrange
        CreateCategoryRequest firstRequest = DataGenerator.CreateIngredientCategory().ToCreateCategoryRequest();
        CreateCategoryRequest secondRequest = DataGenerator.CreateIngredientCategory().ToCreateCategoryRequest();

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

        Category? firstCategory = await apiFactory.MenuDbContext.Category.Query().ById((CategoryId)firstResult.Id, TestContext.Current.CancellationToken);
        Category? secondCategory = await apiFactory.MenuDbContext.Category.Query().ById((CategoryId)secondResult.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(firstCategory);
        Assert.NotNull(secondCategory);
        Assert.Equal(firstCategory.SortOrder + 1, secondCategory.SortOrder);
    }
}
