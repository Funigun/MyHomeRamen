using System.Net;
using System.Net.Http.Json;
using Bogus;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Ingredients;

public sealed class UpdateIngredientTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private Ingredient _ingredient = default!;
    private Ingredient _ingredientA = default!;
    private Ingredient _ingredientB = default!;
    private Category _ingredientCategory = default!;

    public async ValueTask InitializeAsync()
    {
        _ingredientCategory = DataGenerator.GeneratedCategories.First(c => c.CategoryType == CategoryType.Ingredient);
        _ingredient = Ingredient.Create(
            Guid.NewGuid(),
            $"UpdateIngredient_{Guid.NewGuid():N}",
            "Original description text",
            1.50m,
            [_ingredientCategory]);

        apiFactory.MenuDbContext.Ingredient.Add(_ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _ingredientA = Ingredient.Create(
            Guid.NewGuid(),
            $"IngredientA_{Guid.NewGuid():N}",
            "Description for ingredient A",
            1.00m,
            [_ingredientCategory]);

        _ingredientB = Ingredient.Create(
            Guid.NewGuid(),
            $"IngredientB_{Guid.NewGuid():N}",
            "Description for ingredient B",
            2.00m,
            [_ingredientCategory]);

        apiFactory.MenuDbContext.Ingredient.AddRange([_ingredientA, _ingredientB]);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task UpdateIngredient_ShouldReturnOk_ForValidRequest()
    {
        // Arrange
        UpdateIngredientRequest request = new(
            $"Updated_With_Valid_Name",
            "Updated description text here",
            3.99m,
            [_ingredientCategory.Id.Value]);

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{_ingredient.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — 200 returned
        await response.AssertStatusCode(HttpStatusCode.OK);

        UpdateIngredientResponse? result = await response.Content.ReadFromJsonAsync<UpdateIngredientResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(_ingredient.Id.Value, result.Id);

        // Assert — updated fields persisted
        Ingredient? updated = await apiFactory.MenuDbContext.Ingredient.Specification()
            .ById(_ingredient.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(updated);
        Assert.Equal(request.Name, updated.Name);
        Assert.Equal(request.Description, updated.Description);
        Assert.Equal(request.Price, updated.Price);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser()
    {
        // Arrange
        Guid id = _ingredient.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(_ingredient.ToUpdateIngredientRequest());

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Customer)]
    public async Task UpdateIngredient_ShouldReturnForbidden_ForNonAdminRole(UserRoles role)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{_ingredient.Id}")
            .WithJsonContent(_ingredient.ToUpdateIngredientRequest())
            .AddAuthorizationHeader(role);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        Ingredient ingredient = DataGenerator.GeneratedIngredients.First();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{nonExistentId}")
            .WithJsonContent(ingredient.ToUpdateIngredientRequest())
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(InvalidUpdateIngredientRequests), MemberType = typeof(UpdateIngredientTests))]
    public async Task UpdateIngredient_ShouldReturnBadRequest_ForInvalidRequest(UpdateIngredientRequest request)
    {
        // Arrange
        Guid id = _ingredient.Id;

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    public static TheoryData<UpdateIngredientRequest> InvalidUpdateIngredientRequests()
    {
        Faker faker = new();
        string validName = faker.Random.String2(IngredientNameValidator.MinLength, IngredientNameValidator.MaxLength);
        string validDescription = faker.Random.String2(IngredientDescriptionValidator.MinLength, IngredientDescriptionValidator.MaxLength);
        decimal validPrice = faker.Finance.Amount(IngredientPriceValidator.MinPrice, IngredientPriceValidator.MaxPrice);
        IEnumerable<Guid> validCategoryIds = [Guid.NewGuid()];

        return
        [
            new UpdateIngredientRequest(string.Empty, validDescription, validPrice, validCategoryIds),
            new UpdateIngredientRequest(faker.Random.String2(1, IngredientNameValidator.MinLength - 1), validDescription, validPrice, validCategoryIds),
            new UpdateIngredientRequest(faker.Random.String2(IngredientNameValidator.MaxLength + 1, IngredientNameValidator.MaxLength + 10), validDescription, validPrice, validCategoryIds),
            new UpdateIngredientRequest(validName, faker.Random.String2(IngredientDescriptionValidator.MaxLength + 1, IngredientDescriptionValidator.MaxLength + 10), validPrice, validCategoryIds),
            new UpdateIngredientRequest(validName, validDescription, IngredientPriceValidator.MinPrice - 0.01m, validCategoryIds),
            new UpdateIngredientRequest(validName, validDescription, IngredientPriceValidator.MaxPrice + 0.01m, validCategoryIds),
            new UpdateIngredientRequest(validName, validDescription, validPrice, []),
        ];
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnBadRequest_WhenNameAlreadyTakenByDifferentIngredient()
    {
        // Arrange
        UpdateIngredientRequest request = _ingredientA.ToUpdateIngredientRequest() with { Name = _ingredientB.Name };

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{_ingredientA.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnOk_WhenNameIsUnchanged()
    {
        // Arrange
        Category ingredientCategory = DataGenerator.GeneratedCategories
            .First(c => c.CategoryType == CategoryType.Ingredient);

        Ingredient ingredient = Ingredient.Create(
            Guid.NewGuid(),
            $"SameNameTest_{Guid.NewGuid():N}",
            "Description stays the same here",
            2.50m,
            [ingredientCategory]);

        apiFactory.MenuDbContext.Ingredient.Add(ingredient);
        await apiFactory.MenuDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateIngredientRequest request = ingredient.ToUpdateIngredientRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions
            .CreatePutMessage($"/api/menu/ingredients/{ingredient.Id}")
            .WithJsonContent(request)
            .AddAuthorizationHeader(UserRoles.Admin);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert — self-rename passes name-uniqueness check
        await response.AssertStatusCode(HttpStatusCode.OK);
    }
}
