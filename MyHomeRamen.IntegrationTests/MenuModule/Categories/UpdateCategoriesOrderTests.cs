using System.Net;
using MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Categories;

public sealed class UpdateCategoriesOrderTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task UpdateCategoriesOrder_ShouldReturnNoContent_ForValidRequest()
    {
        // Arrange
        List<Category> categories = DataGenerator.GeneratedCategories.ToList();
        List<CategoryOrderItemDto> items = categories
            .Select((c, index) => new CategoryOrderItemDto(c.Id.Value, categories.Count - 1 - index))
            .ToList();

        UpdateCategoriesOrderRequest request = new(items);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage("/api/menu/categories/order")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.NoContent);

        foreach (CategoryOrderItemDto item in items)
        {
            Category? updated = await apiFactory.MenuDbContext.Category.Specification()
                .ById((CategoryId)item.Id, TestContext.Current.CancellationToken);

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
    [MemberData(nameof(DataGenerator.InvalidUpdateCategoriesOrderRequests), MemberType = typeof(DataGenerator))]
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
}
