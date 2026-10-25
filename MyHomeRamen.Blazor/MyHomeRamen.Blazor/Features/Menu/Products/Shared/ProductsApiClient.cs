using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Shared;

public sealed class ProductsApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/products";
    private const string FailureMessage = "Product request failed.";

    public Task<ApiResult<GetProductsForManageResponse>> GetForManageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetForManageCoreAsync(pageNumber, pageSize, cancellationToken));

    public Task<ApiResult<GetProductByIdForManageResponse>> GetByIdForManageAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetByIdForManageCoreAsync(id, cancellationToken));

    public Task<ApiResult<ProductMutationResponse>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateCoreAsync(id, request, cancellationToken));

    public Task<ApiResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => DeleteCoreAsync(id, cancellationToken));

    private async Task<GetProductsForManageResponse> GetForManageCoreAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            $"{BaseUrl}/manage?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetProductsForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetProductByIdForManageResponse> GetByIdForManageCoreAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/{id}/manage", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetProductByIdForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<ProductMutationResponse> CreateCoreAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<ProductMutationResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> UpdateCoreAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }

    private async Task<bool> DeleteCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }
}
