using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

public sealed class RestaurantIngredientPricesApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/ingredient-prices";
    private const string FailureMessage = "Restaurant ingredient price request failed.";

    public Task<ApiResult<GetRestaurantIngredientPricesForManageResponse>> GetForManageAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetForManageCoreAsync(restaurantId, cancellationToken));

    public Task<ApiResult<bool>> SetAsync(
        Guid ingredientId,
        Guid restaurantId,
        SetRestaurantIngredientPriceRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SetCoreAsync(ingredientId, restaurantId, requestBody, cancellationToken));

    public Task<ApiResult<bool>> UpdateAsync(
        Guid ingredientId,
        Guid restaurantId,
        SetRestaurantIngredientPriceRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => UpdateCoreAsync(ingredientId, restaurantId, requestBody, cancellationToken));

    public Task<ApiResult<bool>> RemoveAsync(
        Guid ingredientId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => RemoveCoreAsync(ingredientId, restaurantId, cancellationToken));

    private async Task<GetRestaurantIngredientPricesForManageResponse> GetForManageCoreAsync(
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            $"{BaseUrl}/manage",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetRestaurantIngredientPricesForManageResponse>(
                cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> SetCoreAsync(
        Guid ingredientId,
        Guid restaurantId,
        SetRestaurantIngredientPriceRequest requestBody,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Post,
            BaseUrl,
            restaurantId);
        request.Content = JsonContent.Create(requestBody);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }

    private async Task<bool> UpdateCoreAsync(
        Guid ingredientId,
        Guid restaurantId,
        SetRestaurantIngredientPriceRequest requestBody,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Put,
            $"{BaseUrl}/{ingredientId}",
            restaurantId);
        request.Content = JsonContent.Create(requestBody);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }

    private async Task<bool> RemoveCoreAsync(
        Guid ingredientId,
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Delete,
            $"{BaseUrl}/{ingredientId}",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }
}
