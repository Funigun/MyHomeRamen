using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Menu.Features.Products.GetProductById;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class GetProductByIdTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string EndpointBase = "/api/menu/products";
    private Product _product = default!;

    public async ValueTask InitializeAsync()
    {
        Category ingredientCategory = DataGenerator.CreateIngredientCategory();
        Category productCategory = DataGenerator.CreateProductCategory();
        Ingredient ingredient = DataGenerator.CreateIngredient(ingredientCategory);
        _product = DataGenerator.CreateProduct([ingredient], [], productCategory);

        apiFactory.MenuDbContext.Category.AddRange([ingredientCategory, productCategory]);
        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task GetProductById_ShouldReturnOk_ForAnonymousUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnResponseWithCorrectFields()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(_product.Id.Value, result.Id);
        Assert.Equal(_product.Name, result.Name);
        Assert.Equal(_product.Description, result.Description);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBaseIngredientsWithNameDescriptionAndPrice()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content.ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(_product.BaseIngredients.Count, result.BaseIngredients.Count);

        foreach (Ingredient ingredient in _product.BaseIngredients)
        {
            Assert.Contains(
                result.BaseIngredients,
                dto => dto.Name == ingredient.Name &&
                       dto.Description == ingredient.Description &&
                       dto.Price == ingredient.Price);
        }
    }

    [Fact]
    public async Task GetProductById_ShouldReturnCustomIngredientsWithNameDescriptionAndPrice()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{_product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(_product.CustomIngredients.Count, result.CustomIngredients.Count);

        foreach (Ingredient ingredient in _product.CustomIngredients)
        {
            Assert.Contains(
                result.CustomIngredients,
                dto => dto.Name == ingredient.Name &&
                       dto.Description == ingredient.Description &&
                       dto.Price == ingredient.Price);
        }
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
