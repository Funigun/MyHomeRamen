using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

public sealed class IngredientApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/ingredients";
    private const string FailureMessage = "Ingredient request failed.";

    public Task<ApiResult<GetIngredientsForManageResponse>> GetForManageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetForManageCoreAsync(pageNumber, pageSize, cancellationToken));

    public Task<ApiResult<GetIngredientByIdResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetByIdCoreAsync(id, cancellationToken));

    public Task<ApiResult<GetIngredientsForDropdownResponse>> GetDropdownAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetDropdownCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> CreateAsync(CreateIngredientRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> UpdateAsync(Guid id, UpdateIngredientRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateCoreAsync(id, request, cancellationToken));

    public Task<ApiResult<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => DeleteCoreAsync(id, cancellationToken));

    private async Task<GetIngredientsForManageResponse> GetForManageCoreAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            $"{BaseUrl}/manage?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientsForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetIngredientByIdResponse> GetByIdCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientByIdResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetIngredientsForDropdownResponse> GetDropdownCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/dropdown", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientsForDropdownResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task CreateCoreAsync(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task UpdateCoreAsync(Guid id, UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task DeleteCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }
}
