using System.Net;
using System.Net.Http.Json;
using Bogus;
using MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class UpdateProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private static Product _product = default!;
    private static Product _productA = default!;
    private static Product _productB = default!;
    private static Category _productCategory = default!;
    private static Ingredient _ingredient = default!;

    public async ValueTask InitializeAsync()
    {
        _productCategory = DataGenerator.CreateProductCategory();
        Category ingredientCategory = DataGenerator.CreateIngredientCategory();
        _ingredient = DataGenerator.CreateIngredient(ingredientCategory);
        _product = DataGenerator.CreateProduct([_ingredient], [], _productCategory);
        _productA = DataGenerator.CreateProduct([_ingredient], [], _productCategory);
        _productB = DataGenerator.CreateProduct([_ingredient], [], _productCategory);

        apiFactory.MenuDbContext.Category.Add(_productCategory);
        apiFactory.MenuDbContext.Ingredient.Add(_ingredient);
        apiFactory.MenuDbContext.Product.AddRange([_product, _productA, _productB]);
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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{_product.Id}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{nonExistentId}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{id}");
        httpRequest.WithJsonContent(_product.ToUpdateProductRequest());

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
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{_product.Id}");
        httpRequest.WithJsonContent(_product.ToUpdateProductRequest());
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.GetUser(role));

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

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{_productA.Id}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(InvalidUpdateProductRequests), MemberType = typeof(UpdateProductTests))]
    public async Task UpdateProduct_ShouldReturnBadRequest_ForInvalidRequest(UpdateProductRequest request)
    {
        // Arrange
        Guid id = _product.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/menu/products/{id}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.AdminUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<UpdateProductRequest> InvalidUpdateProductRequests()
    {
        Faker faker = new();
        Guid validCategoryId = _productCategory.Id;
        Guid[] validIngredientIds = [ _ingredient.Id ];
        string validName = faker.Random.String2(ProductConstants.MinNameLength, ProductConstants.MaxNameLength);
        string validDescription = faker.Random.String2(ProductConstants.MinDescriptionLength, ProductConstants.MaxDescriptionLength);
        decimal validPrice = faker.Finance.Amount(ProductConstants.MinPrice, ProductConstants.MaxPrice);

        return
        [
            new UpdateProductRequest(string.Empty, validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(faker.Random.String2(1, ProductConstants.MinNameLength - 1), validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(faker.Random.String2(ProductConstants.MaxNameLength + 1, ProductConstants.MaxNameLength + 10), validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, faker.Random.String2(ProductConstants.MaxDescriptionLength + 1, ProductConstants.MaxDescriptionLength + 10), validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, ProductConstants.MinPrice - 0.01m, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, ProductConstants.MaxPrice + 0.01m, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, validPrice, Guid.Empty, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, [], []),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, validIngredientIds, [Guid.Empty]),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, validIngredientIds, validIngredientIds),
        ];
    }
}
