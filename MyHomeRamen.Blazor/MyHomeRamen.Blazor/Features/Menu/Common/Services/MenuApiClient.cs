using MyHomeRamen.Blazor.Features.Menu.Categories.Requests;
using MyHomeRamen.Blazor.Features.Menu.Categories.Responses;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.Requests;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;
using MyHomeRamen.Blazor.Features.Menu.Products.Requests;
using MyHomeRamen.Blazor.Features.Menu.Products.Responses;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuApiClient(HttpClient httpClient)
{
    public async Task<Guid> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu/products", request, ct);
        response.EnsureSuccessStatusCode();

        CreateProductResponse? result = await response.Content.ReadFromJsonAsync<CreateProductResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize product creation response.");
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu/categories", request, ct);
        response.EnsureSuccessStatusCode();

        CreateCategoryResponse? result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize category creation response.");
    }

    public async Task<Guid> CreateIngredientAsync(CreateIngredientRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu/ingredients", request, ct);
        response.EnsureSuccessStatusCode();

        CreateIngredientResponse? result = await response.Content.ReadFromJsonAsync<CreateIngredientResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize ingredient creation response.");
    }

    public async Task<IEnumerable<GetCategoriesByTypeResponse>> GetCategoriesByTypeAsync(int categoryType, CancellationToken ct = default)
    {
        IEnumerable<GetCategoriesByTypeResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>(
                $"/api/menu/categories/by-type?categoryType={categoryType}", ct);

        return result ?? [];
    }

    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> GetIngredientsForDropdownAsync(CancellationToken ct = default)
    {
        IEnumerable<GetIngredientsForDropdownResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetIngredientsForDropdownResponse>>(
                "/api/menu/ingredients/dropdown", ct);

        return result ?? [];
    }

    public async Task<GetIngredientsForManageResponse?> GetIngredientsForManageAsync(
        string? name = null,
        IEnumerable<Guid>? categoryIds = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        List<string> queryParts = [];

        if (!string.IsNullOrWhiteSpace(name))
        {
            queryParts.Add($"name={Uri.EscapeDataString(name)}");
        }

        if (categoryIds is not null)
        {
            queryParts.AddRange(categoryIds.Select(id => $"categoryIds={id}"));
        }

        queryParts.Add($"pageNumber={pageNumber}");
        queryParts.Add($"pageSize={pageSize}");

        string url = $"/api/menu/ingredients/manage?{string.Join("&", queryParts)}";

        return await httpClient.GetFromJsonAsync<GetIngredientsForManageResponse>(url, ct);
    }

    public async Task<GetIngredientByIdResponse?> GetIngredientByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetIngredientByIdResponse>($"/api/menu/ingredients/{id}", ct);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/menu/categories/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteIngredientAsync(Guid id, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/menu/ingredients/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UpdateIngredientResponse> UpdateIngredientAsync(Guid id, UpdateIngredientRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/menu/ingredients/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UpdateIngredientResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from UpdateIngredient endpoint.");
    }

    public async Task UpdateCategoriesOrderAsync(UpdateCategoriesOrderRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/menu/categories/order", request, ct);
        response.EnsureSuccessStatusCode();
    }
}
