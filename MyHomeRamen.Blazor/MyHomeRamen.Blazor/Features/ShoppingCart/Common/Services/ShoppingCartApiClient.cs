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

    public Task<ApiResult<bool>> RemoveItemAsync(Guid basketId, Guid itemId, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => RemoveItemCoreAsync(basketId, itemId, ct));

    public Task<ApiResult<bool>> ClearBasketAsync(Guid basketId, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => ClearBasketCoreAsync(basketId, ct));

    public Task<ApiResult<bool>> UpdateQuantityAsync(Guid basketId, Guid itemId, int quantity, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdateQuantityCoreAsync(basketId, itemId, quantity, ct));

    public Task<ApiResult<GetCurrentBasketSummaryResponse>> GetCurrentBasketSummaryAsync(CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetCurrentBasketSummaryCoreAsync(ct));

    public Task<ApiResult<GetCurrentBasketDetailsResponse>> GetCurrentBasketDetailsAsync(CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetCurrentBasketDetailsCoreAsync(ct));

    public Task<ApiResult<ShippingDetailsResponse>> GetShippingDetailsAsync(Guid basketId, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetShippingDetailsCoreAsync(basketId, ct));

    public Task<ApiResult<PaymentDetailsResponse>> GetPaymentDetailsAsync(Guid basketId, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => GetPaymentDetailsCoreAsync(basketId, ct));

    public Task<ApiResult<bool>> UpdateShippingDetailsAsync(Guid basketId, UpdateShippingDetailsRequest request, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdateShippingDetailsCoreAsync(basketId, request, ct));

    public Task<ApiResult<bool>> UpdatePaymentDetailsAsync(Guid basketId, UpdatePaymentDetailsRequest request, CancellationToken ct = default) =>
        apiService.ExecuteAsync(() => UpdatePaymentDetailsCoreAsync(basketId, request, ct));

    private async Task<AddItemToBasketResponse> AddItemCoreAsync(AddItemToBasketRequest request, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/shoppingcart/basket/items", request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);

        return await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> RemoveItemCoreAsync(Guid basketId, Guid itemId, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/shoppingcart/baskets/{basketId}/items/{itemId}", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<bool> ClearBasketCoreAsync(Guid basketId, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/shoppingcart/baskets/{basketId}", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<bool> UpdateQuantityCoreAsync(Guid basketId, Guid itemId, int quantity, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync(
            $"/api/shoppingcart/baskets/{basketId}/items/{itemId}/quantity",
            new { Quantity = quantity },
            ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<GetCurrentBasketSummaryResponse> GetCurrentBasketSummaryCoreAsync(CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/shoppingcart/basket/summary", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<GetCurrentBasketSummaryResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetCurrentBasketDetailsResponse> GetCurrentBasketDetailsCoreAsync(CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/shoppingcart/basket/details", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<GetCurrentBasketDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<ShippingDetailsResponse> GetShippingDetailsCoreAsync(Guid basketId, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/shopping-cart/{basketId}/shipping-details", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<ShippingDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> UpdateShippingDetailsCoreAsync(Guid basketId, UpdateShippingDetailsRequest request, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/shopping-cart/{basketId}/update-shipping-details", request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }

    private async Task<PaymentDetailsResponse> GetPaymentDetailsCoreAsync(Guid basketId, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/shopping-cart/{basketId}/payment-details", ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return await response.Content.ReadFromJsonAsync<PaymentDetailsResponse>(ct)
            ?? throw new ApiRequestException(System.Net.HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> UpdatePaymentDetailsCoreAsync(Guid basketId, UpdatePaymentDetailsRequest request, CancellationToken ct)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/shopping-cart/{basketId}/update-payment-details", request, ct);
        await response.EnsureApiSuccessAsync(FailureMessage, ct);
        return true;
    }
}
