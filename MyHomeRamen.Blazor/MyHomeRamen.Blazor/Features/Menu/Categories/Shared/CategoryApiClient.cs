using System.Net;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.DTOs;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Responses;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Shared;

public sealed class CategoryApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/categories";
    private const string FailureMessage = "Category request failed.";

    public Task<ApiResult<GetCategoriesByTypeResponse>> GetByTypeAsync(int categoryType, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetByTypeCoreAsync(categoryType, cancellationToken));

    public Task<ApiResult<bool>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => DeleteCoreAsync(id, cancellationToken));

    public Task<ApiResult<bool>> UpdateOrderAsync(IEnumerable<CategoryOrderItemDto> items, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateOrderCoreAsync(new UpdateCategoriesOrderRequest(items), cancellationToken));

    private async Task<GetCategoriesByTypeResponse> GetByTypeCoreAsync(int categoryType, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            $"{BaseUrl}/by-type?categoryType={categoryType}",
            cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetCategoriesByTypeResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> CreateCoreAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }

    private async Task<bool> DeleteCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }

    private async Task<bool> UpdateOrderCoreAsync(UpdateCategoriesOrderRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/order", request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }
}
