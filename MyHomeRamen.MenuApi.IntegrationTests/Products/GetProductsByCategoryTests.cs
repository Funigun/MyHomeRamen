using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class GetProductsByCategoryTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Category _ingredientCategory = default!;
    private IEnumerable<Category> _productCategories = [];

    public async ValueTask InitializeAsync()
    {
        _productCategories = DataGenerator.CreateProductCategories(3);
        _ingredientCategory = DataGenerator.CreateIngredientCategory();
        Ingredient ingredient = DataGenerator.CreateIngredient(_ingredientCategory);
        Product product = DataGenerator.CreateProduct([ingredient], [], _productCategories.First());
        Product secondProduct = DataGenerator.CreateProduct([ingredient], [], _productCategories.Skip(1).First());
        Product thirdProduct = DataGenerator.CreateProduct([ingredient], [], _productCategories.Skip(1).First());

        apiFactory.MenuDbContext.Category.AddRange(_productCategories);
        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        apiFactory.MenuDbContext.Product.AddRange([product, secondProduct, thirdProduct]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnOkWithProducts_ForValidProductCategory()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/products?categoryId={_productCategories.First().Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsByCategoryResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsByCategoryResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Products);
        Assert.All(result.Products, product =>
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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/products?categoryId={_productCategories.Last().Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductsByCategoryResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductsByCategoryResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Empty(result.Products);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForEmptyCategoryId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/products?categoryId={Guid.Empty}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForNonExistentCategoryId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/products?categoryId={Guid.NewGuid()}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnBadRequest_ForIngredientCategory()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"/api/menu/products?categoryId={_ingredientCategory.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
