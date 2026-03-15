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
}

public sealed record CreateProductResponse(Guid Id);
