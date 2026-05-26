using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Products;

public sealed class GetProductsByCategoryTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task GetProductsByCategory_ShouldReturnOkWithProducts_ForValidProductCategory()
    {
        // Arrange
        Category category = DataGenerator.GeneratedProducts.First().Categories.First(c => c.CategoryType == CategoryType.Product);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/products?categoryId={category.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetProductsByCategoryResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetProductsByCategoryResponse>>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, product =>
        {
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.NotNull(product.Name);
            Assert.NotNull(product.Ingredients);
        });
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnEmptyList_ForValidCategoryWithNoProducts()
    {
        // Arrange
        Category emptyCategory = DataGenerator.GenerateValidCategory(CategoryType.Product);
        apiFactory.MenuDbContext.Categories.Add(emptyCategory);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/products?categoryId={emptyCategory.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        IEnumerable<GetProductsByCategoryResponse>? result = await responseMessage.Content
            .ReadFromJsonAsync<IEnumerable<GetProductsByCategoryResponse>>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForEmptyCategoryId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/products?categoryId={Guid.Empty}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForNonExistentCategoryId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/products?categoryId={Guid.NewGuid()}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForIngredientCategory()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GeneratedCategories.First(c => c.CategoryType == CategoryType.Ingredient);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"/api/menu/products?categoryId={ingredientCategory.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
