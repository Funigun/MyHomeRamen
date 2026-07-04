using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule.Products;

public sealed class UpdateProductTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_ForValidRequest()
    {
        // Arrange
        Category productCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Product);

        Product product = Product.Create(
            Guid.NewGuid(),
            $"UpdateTest_Random_Product_Name",
            "Original product description that is long enough to pass validation.",
            10.0m,
            string.Empty,
            [DataGenerator.GetRandomIngredient()],
            [],
            [productCategory]);

        apiFactory.MenuDbContext.Product.Add(product);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateProductRequest request = new(
            $"UpdateTest_Random_Product_Name_Updated",
            "Updated product description that is long enough to be valid.",
            25.0m,
            productCategory.Id.Value,
            [DataGenerator.GetRandomIngredient().Id],
            []);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{product.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 200 returned
        await response.AssertStatusCode(HttpStatusCode.OK);

        UpdateProductResponse? result = await response.Content.ReadFromJsonAsync<UpdateProductResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(product.Id.Value, result.Id);

        // Assert — updated fields persisted
        Product? updated = await apiFactory.MenuDbContext.Product.Query().ById(product.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(updated);
        Assert.Equal(request.Name, updated.Name);
        Assert.Equal(request.Price, updated.Price);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        UpdateProductRequest request = DataGenerator.GenerateValidProduct().ToUpdateProductRequest();

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
        Guid id = DataGenerator.GeneratedProducts.First().Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{id}")
            .WithJsonContent(DataGenerator.GeneratedProducts.First().ToUpdateProductRequest());

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
        Product product = DataGenerator.GeneratedProducts.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{product.Id}")
            .WithJsonContent(product.ToUpdateProductRequest())
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
        Category productCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Product);

        Product productA = Product.Create(
            Guid.NewGuid(),
            $"ProductA_{Guid.NewGuid():N}",
            "Description for product A that is long enough to pass validation.",
            10.0m,
            string.Empty,
            [DataGenerator.GetRandomIngredient()],
            [],
            [productCategory]);

        Product productB = Product.Create(
            Guid.NewGuid(),
            $"ProductB_{Guid.NewGuid():N}"[..20],
            "Description for product A that is long enough to pass validation.",
            15.0m,
            string.Empty,
            [DataGenerator.GetRandomIngredient()],
            [],
            [productCategory]);

        apiFactory.MenuDbContext.Product.AddRange([productA, productB]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateProductRequest request = productA.ToUpdateProductRequest() with { Name = productB.Name };

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/products/{productA.Id}")
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
        Guid id = DataGenerator.GeneratedProducts.First().Id;

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
