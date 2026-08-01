using System.Net;
using Bogus;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Products;

public sealed class CreateProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Product _product = default!;
    private static Guid _productCategoryId = Guid.Empty;
    private static Guid _productIngredientId = Guid.Empty;

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

        _productCategoryId = productCategory.Id;
        _productIngredientId = ingredient.Id;
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task CreateProduct_ShouldReturnLocationHeader_ForValidRequest()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
        Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present in the response.");
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnNotAuthorized_ForNotAuthenticatedUser()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task CreateProduct_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Forbidden;

        CreateProductRequest request = _product.ToCreateProductRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidCreateProductRequests), MemberType = typeof(CreateProductTests))]
    public async Task CreateProduct_ShouldReturnBadRequest_ForInvalidRequest(CreateProductRequest request)
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
                                                                   .WithJsonContent(request)
                                                                   .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await responseMessage.AssertStatusCode(expectedStatusCode);
    }

    public static TheoryData<CreateProductRequest> InvalidCreateProductRequests()
    {
        Faker faker = new();
        Guid validCategoryId = _productCategoryId;
        Guid[] validIngredientIds = [_productIngredientId];
        string validName = faker.Random.String2(ProductConstants.MinNameLength, ProductConstants.MaxNameLength);
        string validDescription = faker.Random.String2(ProductConstants.MinDescriptionLength, ProductConstants.MaxDescriptionLength);

        return
        [
            new CreateProductRequest(string.Empty, validDescription, ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(faker.Random.String2(1, ProductConstants.MinNameLength - 1), validDescription, ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(faker.Random.String2(ProductConstants.MaxNameLength + 1, ProductConstants.MaxNameLength + 10), validDescription, ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, string.Empty, ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, faker.Random.String2(1, ProductConstants.MinDescriptionLength - 1), ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, faker.Random.String2(ProductConstants.MaxDescriptionLength + 1, ProductConstants.MaxDescriptionLength + 10), ProductConstants.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductConstants.MinPrice - 0.01m, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductConstants.MaxPrice + 0.01m, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductConstants.MinPrice, Guid.Empty, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductConstants.MinPrice, validCategoryId, [], []),
            new CreateProductRequest(validName, validDescription, ProductConstants.MinPrice, validCategoryId, validIngredientIds, [Guid.Empty]),
            new CreateProductRequest(validName, validDescription, ProductConstants.MinPrice, validCategoryId, validIngredientIds, validIngredientIds),
        ];
    }
}
