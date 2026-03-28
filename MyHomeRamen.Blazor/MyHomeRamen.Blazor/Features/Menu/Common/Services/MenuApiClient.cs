using MyHomeRamen.Blazor.Features.Menu.Categories.CreateCategory;
using MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

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

    public async Task<IEnumerable<GetCategoriesForDropdownResponse>> GetCategoriesForDropdownAsync(
        int categoryType,
        CancellationToken ct = default)
    {
        IEnumerable<GetCategoriesForDropdownResponse>? result = await httpClient
            .GetFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>(
                $"/api/menu/categories/dropdown?categoryType={categoryType}", ct);

        return result ?? [];
    }
}

public sealed record CreateProductResponse(Guid Id);

public sealed record CreateCategoryResponse(Guid Id);

public sealed record GetCategoriesForDropdownResponse(Guid Id, string Name);
