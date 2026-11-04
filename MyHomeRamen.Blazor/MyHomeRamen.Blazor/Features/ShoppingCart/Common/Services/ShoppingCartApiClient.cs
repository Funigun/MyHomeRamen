using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;

public sealed class ShoppingCartApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string FailureMessage = "Shopping cart request failed.";

    public Task<ApiResult<AddItemToBasketResponse>> AddItemToBasketAsync(AddItemToBasketRequest request, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => AddItemCoreAsync(request, ct));

    public Task<ApiResult<bool>> RemoveItemAsync(
        Guid basketId,
        Guid itemId,
        Guid restaurantId,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => RemoveItemCoreAsync(basketId, itemId, restaurantId, ct));

    public Task<ApiResult<bool>> ClearBasketAsync(Guid basketId, Guid restaurantId, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => ClearBasketCoreAsync(basketId, restaurantId, ct));

    public Task<ApiResult<bool>> UpdateQuantityAsync(
        Guid basketId,
        Guid itemId,
        int quantity,
        Guid restaurantId,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdateQuantityCoreAsync(basketId, itemId, quantity, restaurantId, ct));

    public Task<ApiResult<GetCurrentBasketSummaryResponse>> GetCurrentBasketSummaryAsync(CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetCurrentBasketSummaryCoreAsync(ct));

    public Task<ApiResult<GetCurrentBasketDetailsResponse>> GetCurrentBasketDetailsAsync(CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetCurrentBasketDetailsCoreAsync(ct));

    public Task<ApiResult<ShippingDetailsResponse>> GetShippingDetailsAsync(
        Guid basketId,
        Guid restaurantId,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetShippingDetailsCoreAsync(basketId, restaurantId, ct));

    public Task<ApiResult<PaymentDetailsResponse>> GetPaymentDetailsAsync(
        Guid basketId,
        Guid restaurantId,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetPaymentDetailsCoreAsync(basketId, restaurantId, ct));

    public Task<ApiResult<bool>> UpdateShippingDetailsAsync(
        Guid basketId,
        Guid restaurantId,
        UpdateShippingDetailsRequest request,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdateShippingDetailsCoreAsync(basketId, restaurantId, request, ct));

    public Task<ApiResult<bool>> UpdatePaymentDetailsAsync(
        Guid basketId,
        Guid restaurantId,
        UpdatePaymentDetailsRequest request,
        CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdatePaymentDetailsCoreAsync(basketId, restaurantId, request, ct));

    private async Task<AddItemToBasketResponse> AddItemCoreAsync(AddItemToBasketRequest request, CancellationToken ct)
    {
        using HttpRequestMessage httpRequest = RestaurantContextApiRequest.Create(
            HttpMethod.Post,
            "/api/shopping-cart/basket/items",
            request.RestaurantId);
        httpRequest.Content = JsonContent.Create(request);
        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);

        return await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> RemoveItemCoreAsync(Guid basketId, Guid itemId, Guid restaurantId, CancellationToken ct)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Delete,
            $"/api/shopping-cart/baskets/{basketId}/items/{itemId}",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<bool> ClearBasketCoreAsync(Guid basketId, Guid restaurantId, CancellationToken ct)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Delete,
            $"/api/shopping-cart/baskets/{basketId}",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<bool> UpdateQuantityCoreAsync(
        Guid basketId,
        Guid itemId,
        int quantity,
        Guid restaurantId,
        CancellationToken ct)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Put,
            $"/api/shopping-cart/baskets/{basketId}/items/{itemId}/quantity",
            restaurantId);
        request.Content = JsonContent.Create(new { Quantity = quantity });
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<GetCurrentBasketSummaryResponse> GetCurrentBasketSummaryCoreAsync(CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/shopping-cart/basket/summary", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetCurrentBasketDetailsResponse> GetCurrentBasketDetailsCoreAsync(CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/shopping-cart/basket/details", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<ShippingDetailsResponse> GetShippingDetailsCoreAsync(Guid basketId, Guid restaurantId, CancellationToken ct)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            $"/api/shopping-cart/baskets/{basketId}/shipping-details",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<ShippingDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> UpdateShippingDetailsCoreAsync(
        Guid basketId,
        Guid restaurantId,
        UpdateShippingDetailsRequest request,
        CancellationToken ct)
    {
        using HttpRequestMessage httpRequest = RestaurantContextApiRequest.Create(
            HttpMethod.Put,
            $"/api/shopping-cart/baskets/{basketId}/shipping-details",
            restaurantId);
        httpRequest.Content = JsonContent.Create(request);
        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<PaymentDetailsResponse> GetPaymentDetailsCoreAsync(Guid basketId, Guid restaurantId, CancellationToken ct)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            $"/api/shopping-cart/baskets/{basketId}/payment-details",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<PaymentDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> UpdatePaymentDetailsCoreAsync(
        Guid basketId,
        Guid restaurantId,
        UpdatePaymentDetailsRequest request,
        CancellationToken ct)
    {
        using HttpRequestMessage httpRequest = RestaurantContextApiRequest.Create(
            HttpMethod.Put,
            $"/api/shopping-cart/baskets/{basketId}/payment-details",
            restaurantId);
        httpRequest.Content = JsonContent.Create(request);
        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }
}
