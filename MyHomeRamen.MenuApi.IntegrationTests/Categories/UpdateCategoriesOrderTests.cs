using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class UpdateCategoriesOrderTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanEditCategory];
    private (string KeycloakUserId, Guid UserId) _userId;

    public async ValueTask InitializeAsync()
    {
        _userId = await apiFactory.IdentityTestData.SeedUser(_requiredPermissions, "update-categories-user");
    }

    public async ValueTask DisposeAsync()
    {
        await apiFactory.IdentityTestData.DeleteUser(_userId.UserId);
    }

    [Fact]
    public async Task UpdateCategoriesOrder_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        IEnumerable<Category> categories = DataGenerator.CreateProductCategories(5);
        apiFactory.MenuDbContext.Category.AddRange(categories);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        List<CategoryOrderItemDto> items = categories.Select((c, index) => new CategoryOrderItemDto(c.Id.Value, categories.Count() - 1 - index)).ToList();
        UpdateCategoriesOrderRequest request = new(items);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.NoContent);

        using HttpRequestMessage assertRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/categories/by-type?categoryType={(int)CategoryType.Product}");
        assertRequest.AddAuthorizationHeader(_userId);

        HttpResponseMessage assertResponse = await apiFactory.HttpClient.SendAsync(assertRequest, TestContext.Current.CancellationToken);
        GetCategoriesByTypeResponse? updatedCategories = await assertResponse.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken);

        foreach (CategoryOrderItemDto item in items)
        {
            CategoryByTypeDto? updated = updatedCategories?.Categories.FirstOrDefault(c => c.Id == item.Id);

            Assert.NotNull(updated);
            Assert.Equal(item.SortOrder, updated.SortOrder);
        }
    }

    [Fact]
    public async Task UpdateCategoriesOrder_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        List<CategoryOrderItemDto> items = [new CategoryOrderItemDto(Guid.NewGuid(), 0)];
        UpdateCategoriesOrderRequest request = new(items);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order");
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task UpdateCategoriesOrder_ShouldReturnForbidden_ForNonManagerRoles(UserRoles role)
    {
        // Arrange
        List<CategoryOrderItemDto> items = [new CategoryOrderItemDto(Guid.NewGuid(), 0)];
        UpdateCategoriesOrderRequest request = new(items);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(InvalidUpdateCategoriesOrderRequests), MemberType = typeof(UpdateCategoriesOrderTests))]
    public async Task UpdateCategoriesOrder_ShouldReturnBadRequest_ForInvalidRequest(UpdateCategoriesOrderRequest request)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<UpdateCategoriesOrderRequest> InvalidUpdateCategoriesOrderRequests()
    {
        Guid validId = Guid.NewGuid();

        return
        [
            // Empty list
            new UpdateCategoriesOrderRequest([]),

            // Sort order below minimum
            new UpdateCategoriesOrderRequest([new CategoryOrderItemDto(validId, CategoryConstants.MinSortOrder - 1)]),

            // Duplicate IDs
            new UpdateCategoriesOrderRequest([
                new CategoryOrderItemDto(validId, CategoryConstants.MinSortOrder),
                new CategoryOrderItemDto(validId, CategoryConstants.MinSortOrder + 1),
            ]),
        ];
    }
}
