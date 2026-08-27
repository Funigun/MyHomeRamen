using System.Net;
using System.Net.Http.Json;
using Bogus;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class CreateCategoryTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Category _productCategory = default!;
    private Category _prodcutCategoryDuplicateCheck = default!;

    public async ValueTask InitializeAsync()
    {
        _productCategory = DataGenerator.CreateProductCategory("New product category");
        _prodcutCategoryDuplicateCheck = DataGenerator.CreateProductCategory();

        apiFactory.MenuDbContext.Category.Add(_prodcutCategoryDuplicateCheck);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        apiFactory.MenuDbContext.Category.Delete(_prodcutCategoryDuplicateCheck);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnCreated_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;
        CreateCategoryRequest request = _productCategory.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        httpRequest.WithJsonContent(request);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

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
            new CreateCategoryRequest(faker.Random.String2(1, CategoryConstants.MinNameLength - 1), validCategoryType),

            // Name: too long
            new CreateCategoryRequest(faker.Random.String2(CategoryConstants.MaxNameLength + 1, CategoryConstants.MaxNameLength + 10), validCategoryType),

            // CategoryType: invalid
            new CreateCategoryRequest(faker.Random.String2(CategoryConstants.MinNameLength, CategoryConstants.MaxNameLength), 999),
        ];
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnBadRequest_ForDuplicateName()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        CreateCategoryRequest request = _prodcutCategoryDuplicateCheck.ToCreateCategoryRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

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

        using HttpRequestMessage firstHttpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        firstHttpRequest.WithJsonContent(firstRequest);
        firstHttpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        using HttpRequestMessage secondHttpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        secondHttpRequest.WithJsonContent(secondRequest);
        secondHttpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage firstResponse = await apiFactory.HttpClient.SendAsync(firstHttpRequest, TestContext.Current.CancellationToken);
        HttpResponseMessage secondResponse = await apiFactory.HttpClient.SendAsync(secondHttpRequest, TestContext.Current.CancellationToken);

        CreateCategoryResponse firstResult = await firstResponse.ResponseToDto<CreateCategoryResponse>();
        CreateCategoryResponse secondResult = await secondResponse.ResponseToDto<CreateCategoryResponse>();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/categories/by-type?categoryType={(int)CategoryType.Ingredient}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetCategoriesByTypeResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(result.Categories, c => c.Id == firstResult.Id);
        Assert.Contains(result.Categories, c => c.Id == secondResult.Id);

        CategoryByTypeDto firstCategory = result.Categories.First(c => c.Id == firstResult.Id);
        CategoryByTypeDto secondCategory = result.Categories.First(c => c.Id == secondResult.Id);

        Assert.Equal(firstCategory.SortOrder + 1, secondCategory.SortOrder);
    }
}
