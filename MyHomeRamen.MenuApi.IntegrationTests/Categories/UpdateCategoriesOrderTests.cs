using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Categories;

public sealed class UpdateCategoriesOrderTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task UpdateCategoriesOrder_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        IEnumerable<Category> categories = DataGenerator.CreateProductCategories(5);
        apiFactory.MenuDbContext.Category.AddRange(categories);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        List<CategoryOrderItemDto> items = categories.Select((c, index) => new CategoryOrderItemDto(c.Id.Value, categories.Count() - 1 - index)).ToList();
        UpdateCategoriesOrderRequest request = new(items);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.NoContent);

        using HttpRequestMessage assertRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/categories/by-type?categoryType={(int)CategoryType.Product}")
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order")
                                                                   .WithJsonContent(request);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

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
            new UpdateCategoriesOrderRequest([new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder - 1)]),

            // Duplicate IDs
            new UpdateCategoriesOrderRequest([
                new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder),
                new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder + 1),
            ]),
        ];
    }
}
