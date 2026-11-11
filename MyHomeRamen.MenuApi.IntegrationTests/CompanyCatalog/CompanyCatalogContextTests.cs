using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;
using MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;
using MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;
using MyHomeRamen.Features.Menu.Features.Products.CreateProduct;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;

namespace MyHomeRamen.MenuApi.IntegrationTests.CompanyCatalog;

public sealed class CompanyCatalogContextTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private readonly string[] _companyOwnerPermissions =
    [
        PermissionConstants.CanManageProducts,
        PermissionConstants.CanAddProduct,
        PermissionConstants.CanManageIngredients,
        PermissionConstants.CanAddIngredient,
        PermissionConstants.CanManageCategories,
        PermissionConstants.CanAddCategory
    ];

    private (string KeycloakUserId, Guid UserId) _companyOwner;

    public async ValueTask InitializeAsync()
    {
        _companyOwner = await apiFactory.IdentityTestData.SeedUser(
            _companyOwnerPermissions,
            $"company-catalog-owner-{Guid.CreateVersion7():N}");
    }

    public async ValueTask DisposeAsync()
        => await apiFactory.IdentityTestData.DeleteUser(_companyOwner.UserId);

    [Fact]
    public async Task GetCompanyCatalog_ShouldRequireAuthentication_WithoutRestaurantContext()
    {
        using HttpClient client = apiFactory.CreateClient();
        using HttpRequestMessage request =
            HttpClientExtensions.CreateGetMessage("/api/menu/products/manage");

        using HttpResponseMessage response =
            await client.SendAsync(request, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompanyOwner_ShouldLoadEmptyCatalogAndCreateEntries_WithoutRestaurantContext()
    {
        using HttpClient client = apiFactory.CreateClient();

        using HttpRequestMessage productsRequest =
            HttpClientExtensions.CreateGetMessage("/api/menu/products/manage");
        productsRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage productsResponse =
            await client.SendAsync(productsRequest, TestContext.Current.CancellationToken);
        await productsResponse.AssertStatusCode(HttpStatusCode.OK);
        GetProductsForManageResponse? products =
            await productsResponse.Content.ReadFromJsonAsync<GetProductsForManageResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(products);
        Assert.Empty(products.Products);
        Assert.True(products.AllowedActions.CanCreate);

        using HttpRequestMessage ingredientsRequest =
            HttpClientExtensions.CreateGetMessage("/api/menu/ingredients/manage");
        ingredientsRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage ingredientsResponse =
            await client.SendAsync(ingredientsRequest, TestContext.Current.CancellationToken);
        await ingredientsResponse.AssertStatusCode(HttpStatusCode.OK);
        GetIngredientsForManageResponse? ingredients =
            await ingredientsResponse.Content.ReadFromJsonAsync<GetIngredientsForManageResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(ingredients);
        Assert.Empty(ingredients.Ingredients);
        Assert.True(ingredients.AllowedActions.CanCreate);

        using HttpRequestMessage categoriesRequest = HttpClientExtensions.CreateGetMessage(
            $"/api/menu/categories/by-type?categoryType={(int)CategoryType.Product}");
        categoriesRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage categoriesResponse =
            await client.SendAsync(categoriesRequest, TestContext.Current.CancellationToken);
        await categoriesResponse.AssertStatusCode(HttpStatusCode.OK);
        GetCategoriesByTypeResponse? categories =
            await categoriesResponse.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(categories);
        Assert.Empty(categories.Categories);
        Assert.True(categories.AllowedActions.CanCreate);

        string suffix = Guid.CreateVersion7().ToString("N");
        string shortSuffix = suffix[..12];
        using HttpRequestMessage createProductCategoryRequest =
            HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        createProductCategoryRequest.WithJsonContent(
            new CreateCategoryRequest($"Product category {shortSuffix}", (int)CategoryType.Product));
        createProductCategoryRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage createProductCategoryResponse =
            await client.SendAsync(createProductCategoryRequest, TestContext.Current.CancellationToken);
        await createProductCategoryResponse.AssertStatusCode(HttpStatusCode.Created);
        CreateCategoryResponse? productCategory =
            await createProductCategoryResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(productCategory);

        using HttpRequestMessage createIngredientCategoryRequest =
            HttpClientExtensions.CreatePostMessage("/api/menu/categories");
        createIngredientCategoryRequest.WithJsonContent(
            new CreateCategoryRequest($"Ingredient category {shortSuffix}", (int)CategoryType.Ingredient));
        createIngredientCategoryRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage createIngredientCategoryResponse =
            await client.SendAsync(createIngredientCategoryRequest, TestContext.Current.CancellationToken);
        await createIngredientCategoryResponse.AssertStatusCode(HttpStatusCode.Created);
        CreateCategoryResponse? ingredientCategory =
            await createIngredientCategoryResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(ingredientCategory);

        using HttpRequestMessage createIngredientRequest =
            HttpClientExtensions.CreatePostMessage("/api/menu/ingredients");
        createIngredientRequest.WithJsonContent(
            new CreateIngredientRequest($"Ingredient {shortSuffix}", "Catalog ingredient", [ingredientCategory.Id]));
        createIngredientRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage createIngredientResponse =
            await client.SendAsync(createIngredientRequest, TestContext.Current.CancellationToken);
        await createIngredientResponse.AssertStatusCode(HttpStatusCode.Created);
        CreateIngredientResponse? ingredient =
            await createIngredientResponse.Content.ReadFromJsonAsync<CreateIngredientResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(ingredient);

        using HttpRequestMessage createProductRequest =
            HttpClientExtensions.CreatePostMessage("/api/menu/products");
        createProductRequest.WithJsonContent(
            new CreateProductRequest(
                $"Product {shortSuffix}",
                "Catalog product",
                productCategory.Id,
                [ingredient.Id],
                []));
        createProductRequest.AddAuthorizationHeader(_companyOwner);
        using HttpResponseMessage createProductResponse =
            await client.SendAsync(createProductRequest, TestContext.Current.CancellationToken);
        await createProductResponse.AssertStatusCode(HttpStatusCode.Created);
    }
}
