using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Products;

public sealed class GetProductByIdTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/menu/products";

    [Fact]
    public async Task GetProductById_ShouldReturnOk_ForAnonymousUser()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnResponseWithCorrectFields()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First();
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(product.Id.Value, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Description, result.Description);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBaseIngredientsWithNameDescriptionAndPrice()
    {
        // Arrange
        Product product = DataGenerator.GeneratedProducts.First(p => p.BaseIngredients.Count > 0);
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(product.BaseIngredients.Count, result.BaseIngredients.Count);

        foreach (Domain.Menu.Ingredients.Ingredient ingredient in product.BaseIngredients)
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
        Product product = DataGenerator.GeneratedProducts.First(p => p.CustomIngredients.Count > 0);
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage($"{EndpointBase}/{product.Id.Value}");

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
        GetProductByIdResponse? result = await responseMessage.Content
            .ReadFromJsonAsync<GetProductByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.OK);
        Assert.NotNull(result);
        Assert.Equal(product.CustomIngredients.Count, result.CustomIngredients.Count);

        foreach (Domain.Menu.Ingredients.Ingredient ingredient in product.CustomIngredients)
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
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}")
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
