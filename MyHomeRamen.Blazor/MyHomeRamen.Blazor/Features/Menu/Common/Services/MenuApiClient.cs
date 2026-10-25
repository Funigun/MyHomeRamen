using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Responses;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Responses;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;

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

    public async Task<GetCategoriesByTypeResponse> GetCategoriesByTypeAsync(int categoryType, CancellationToken ct = default)
    {
        GetCategoriesByTypeResponse? result = await httpClient
            .GetFromJsonAsync<GetCategoriesByTypeResponse>(
                $"/api/menu/categories/by-type?categoryType={categoryType}", ct);

        return result ?? new([], new CategoryAllowedActions(false, false, false));
    }

    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> GetIngredientsForDropdownAsync(CancellationToken ct = default)
    {
        IEnumerable<GetIngredientsForDropdownResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetIngredientsForDropdownResponse>>(
                "/api/menu/ingredients/dropdown", ct);

        return result ?? [];
    }

    public async Task<GetProductsForManageResponse?> GetProductsForManageAsync(
        string? name = null,
        IEnumerable<Guid>? categoryIds = null,
        IEnumerable<Guid>? ingredientIds = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        string? orderBy = null,
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

        if (ingredientIds is not null)
        {
            queryParts.AddRange(ingredientIds.Select(id => $"ingredientIds={id}"));
        }

        if (priceFrom.HasValue)
        {
            queryParts.Add($"priceFrom={priceFrom.Value}");
        }

        if (priceTo.HasValue)
        {
            queryParts.Add($"priceTo={priceTo.Value}");
        }

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            queryParts.Add($"orderBy={Uri.EscapeDataString(orderBy)}");
        }

        queryParts.Add($"pageNumber={pageNumber}");
        queryParts.Add($"pageSize={pageSize}");

        string url = $"/api/menu/products/manage?{string.Join("&", queryParts)}";

        return await httpClient.GetFromJsonAsync<GetProductsForManageResponse>(url, ct);
    }

    public async Task<GetProductByIdForManageResponse?> GetProductByIdForManageAsync(Guid id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetProductByIdForManageResponse>($"/api/menu/products/{id}/manage", ct);
    }

    public async Task<GetProductByIdResponse?> GetProductByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetProductByIdResponse>($"/api/menu/products/{id}", ct);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/menu/categories/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCategoriesOrderAsync(UpdateCategoriesOrderRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/menu/categories/order", request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UpdateProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/menu/products/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UpdateProductResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from UpdateProduct endpoint.");
    }

    public async Task<IEnumerable<GetMenuCategoriesResponse>> GetMenuCategoriesAsync(CancellationToken ct = default)
    {
        IEnumerable<GetMenuCategoriesResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetMenuCategoriesResponse>>("/api/menu/categories/menu", ct);

        return result ?? [];
    }

    public async Task<IEnumerable<GetProductsByCategoryResponse>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        IEnumerable<GetProductsByCategoryResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetProductsByCategoryResponse>>($"/api/menu/products?categoryId={categoryId}", ct);

        return result ?? [];
    }
}
