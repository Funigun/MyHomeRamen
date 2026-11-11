using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

public sealed class IngredientApiClient(
    HttpClient httpClient,
    ApiService apiService)
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
        using HttpRequestMessage request = new(
            HttpMethod.Get,
            $"{BaseUrl}/manage?pageNumber={pageNumber}&pageSize={pageSize}");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientsForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetIngredientByIdResponse> GetByIdCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, $"{BaseUrl}/{id}");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientByIdResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetIngredientsForDropdownResponse> GetDropdownCoreAsync(CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, $"{BaseUrl}/dropdown");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetIngredientsForDropdownResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task CreateCoreAsync(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, BaseUrl);
        httpRequest.Content = JsonContent.Create(request);
        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task UpdateCoreAsync(Guid id, UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Put, $"{BaseUrl}/{id}");
        httpRequest.Content = JsonContent.Create(request);
        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task DeleteCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Delete, $"{BaseUrl}/{id}");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

}
