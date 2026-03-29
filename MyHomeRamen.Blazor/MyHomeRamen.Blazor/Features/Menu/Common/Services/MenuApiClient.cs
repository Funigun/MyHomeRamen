using MyHomeRamen.Blazor.Features.Menu.Categories.CreateCategory;
using MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.CreateIngredient;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuApiClient(HttpClient httpClient)
{
    public async Task<Guid> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.products", request, ct);
        response.EnsureSuccessStatusCode();

        CreateProductResponse? result = await response.Content.ReadFromJsonAsync<CreateProductResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize product creation response.");
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.categories", request, ct);
        response.EnsureSuccessStatusCode();

        CreateCategoryResponse? result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize category creation response.");
    }

    public async Task<Guid> CreateIngredientAsync(CreateIngredientRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.ingredients", request, ct);
        response.EnsureSuccessStatusCode();

        CreateIngredientResponse? result = await response.Content.ReadFromJsonAsync<CreateIngredientResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize ingredient creation response.");
    }

    public async Task<IEnumerable<GetCategoriesForDropdownResponse>> GetCategoriesForDropdownAsync(
        int categoryType,
        CancellationToken ct = default)
    {
        IEnumerable<GetCategoriesForDropdownResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>(
                $"/api/menu/categories/dropdown?categoryType={categoryType}", ct);

        return result ?? [];
    }

    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> GetIngredientsForDropdownAsync(
        CancellationToken ct = default)
    {
        IEnumerable<GetIngredientsForDropdownResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetIngredientsForDropdownResponse>>(
                "/api/menu/ingredients/dropdown", ct);

        return result ?? [];
    }
}

public sealed record CreateProductResponse(Guid Id);

public sealed record CreateCategoryResponse(Guid Id);

public sealed record CreateIngredientResponse(Guid Id);

public sealed record GetCategoriesForDropdownResponse(Guid Id, string Name);

public sealed record GetIngredientsForDropdownResponse(Guid Id, string Name);
