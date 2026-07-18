using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class UpdateProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Product _product = default!;
    private Product _productA = default!;
    private Product _productB = default!;
    private Category _productCategory = default!;
    private Ingredient _ingredient = default!;

    public async ValueTask InitializeAsync()
    {
        _productCategory = DataGenerator.GenerateValidCategory(CategoryType.Product);
        _ingredient = Ingredient.Create(
            Guid.NewGuid(),
            $"UpdateProduct_{Guid.NewGuid():N}",
            "Ingredient description that is long enough to be valid.",
            1.50m,
            [DataGenerator.GeneratedCategories.First(c => c.CategoryType == CategoryType.Ingredient)]);

        _product = Product.Create(
            Guid.NewGuid(),
            $"UpdateProduct_{Guid.NewGuid():N}",
            "Original product description that is long enough to pass validation.",
            10.0m,
            string.Empty,
            [_ingredient],
            [],
            [_productCategory]);

        apiFactory.MenuDbContext.Product.Add(_product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _productA = Product.Create(
            Guid.NewGuid(),
            $"ProductA_{Guid.NewGuid():N}",
            "Description for product A that is long enough to pass validation.",
            10.0m,
            string.Empty,
            [_ingredient],
            [],
            [_productCategory]);

        _productB = Product.Create(
            Guid.NewGuid(),
            $"ProductB_{Guid.NewGuid():N}"[..20],
            "Description for product B that is long enough to pass validation.",
            15.0m,
            string.Empty,
            [_ingredient],
            [],
            [_productCategory]);

        apiFactory.MenuDbContext.Product.AddRange([_productA, _productB]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_ForValidRequest()
    {
        // Arrange
        UpdateProductRequest request = new(
            $"UpdateTest_Random_Product_Name_Updated",
            "Updated product description that is long enough to be valid.",
            25.0m,
            _productCategory.Id.Value,
            [_ingredient.Id],
            []);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{_product.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 200 returned
        await response.AssertStatusCode(HttpStatusCode.OK);

        UpdateProductResponse? result = await response.Content.ReadFromJsonAsync<UpdateProductResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(_product.Id.Value, result.Id);

        // Assert — updated fields persisted
        Product? updated = await apiFactory.MenuDbContext.Product.Query().ById(_product.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(updated);
        Assert.Equal(request.Name, updated.Name);
        Assert.Equal(request.Price, updated.Price);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        UpdateProductRequest request = _product.ToUpdateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{nonExistentId}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = _product.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{id}")
            .WithJsonContent(_product.ToUpdateProductRequest());

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task UpdateProduct_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{_product.Id}")
            .WithJsonContent(_product.ToUpdateProductRequest())
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenNameAlreadyExistsOnAnotherProduct()
    {
        // Arrange
        UpdateProductRequest request = _productA.ToUpdateProductRequest() with { Name = _productB.Name };

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{_productA.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateProductRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateProduct_ShouldReturnBadRequest_ForInvalidRequest(UpdateProductRequest request)
    {
        // Arrange
        Guid id = _product.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
