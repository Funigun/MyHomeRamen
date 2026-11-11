using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

public sealed class MenuCardsApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/cards";
    private const string FailureMessage = "Menu card request failed.";
    private const int ManagementPageSize = 100;

    public Task<ApiResult<GetMenuCardOptionsResponse>> GetOptionsAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetOptionsCoreAsync(restaurantId, cancellationToken));

    public Task<ApiResult<MenuCardResponseContract>> GetByIdAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetByIdCoreAsync(menuCardId, restaurantId, cancellationToken));

    public Task<ApiResult<GetMenuCardsForManageResponse>> GetForManageAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetForManageCoreAsync(restaurantId, cancellationToken));

    public Task<ApiResult<GetMenuCardForManageResponse>> GetByIdForManageAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetByIdForManageCoreAsync(menuCardId, restaurantId, cancellationToken));

    public Task<ApiResult<MenuCardMutationResponse>> CreateAsync(
        Guid restaurantId,
        CreateMenuCardRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantAsync<MenuCardMutationResponse>(
            HttpMethod.Post,
            BaseUrl,
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> UpdateAsync(
        Guid menuCardId,
        Guid restaurantId,
        UpdateMenuCardRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/{menuCardId}",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> DeactivateAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Delete,
            $"{BaseUrl}/{menuCardId}",
            restaurantId,
            null,
            cancellationToken));

    public Task<ApiResult<bool>> ActivateAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/{menuCardId}/activate",
            restaurantId,
            null,
            cancellationToken));

    public Task<ApiResult<bool>> UpdateOrderAsync(
        Guid restaurantId,
        UpdateMenuCardsOrderRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/order",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<MenuCardMutationResponse>> CreateSectionAsync(
        Guid menuCardId,
        Guid restaurantId,
        CreateMenuSectionRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantAsync<MenuCardMutationResponse>(
            HttpMethod.Post,
            $"{BaseUrl}/{menuCardId}/sections",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> UpdateSectionAsync(
        Guid menuCardId,
        Guid sectionId,
        Guid restaurantId,
        UpdateMenuSectionRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/{menuCardId}/sections/{sectionId}",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> DeleteSectionAsync(
        Guid menuCardId,
        Guid sectionId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Delete,
            $"{BaseUrl}/{menuCardId}/sections/{sectionId}",
            restaurantId,
            null,
            cancellationToken));

    public Task<ApiResult<MenuCardMutationResponse>> AddOfferAsync(
        Guid menuCardId,
        Guid sectionId,
        Guid restaurantId,
        AddMenuCardOfferRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantAsync<MenuCardMutationResponse>(
            HttpMethod.Post,
            $"{BaseUrl}/{menuCardId}/sections/{sectionId}/offers",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> UpdateOfferAsync(
        Guid menuCardId,
        Guid sectionId,
        Guid listingId,
        Guid restaurantId,
        UpdateMenuCardOfferRequest requestBody,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/{menuCardId}/sections/{sectionId}/offers/{listingId}",
            restaurantId,
            requestBody,
            cancellationToken));

    public Task<ApiResult<bool>> RemoveOfferAsync(
        Guid menuCardId,
        Guid sectionId,
        Guid listingId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => SendForRestaurantNoContentAsync(
            HttpMethod.Delete,
            $"{BaseUrl}/{menuCardId}/sections/{sectionId}/offers/{listingId}",
            restaurantId,
            null,
            cancellationToken));

    private async Task<GetMenuCardOptionsResponse> GetOptionsCoreAsync(
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            $"{BaseUrl}/options?restaurantId={Uri.EscapeDataString(restaurantId.ToString())}",
            cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetMenuCardOptionsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<MenuCardResponseContract> GetByIdCoreAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await GetForRestaurantAsync<MenuCardResponseContract>(
            $"{BaseUrl}/{menuCardId}",
            restaurantId,
            cancellationToken);

    private async Task<GetMenuCardsForManageResponse> GetForManageCoreAsync(
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        GetMenuCardsForManageResponse firstPage = await GetForRestaurantAsync<GetMenuCardsForManageResponse>(
            $"{BaseUrl}/manage?pageNumber=1&pageSize={ManagementPageSize}",
            restaurantId,
            cancellationToken);
        List<MenuCardForManageContract> items = firstPage.Items.ToList();
        int pageCount = (firstPage.TotalCount + firstPage.PageSize - 1) / firstPage.PageSize;

        for (int pageNumber = 2; pageNumber <= pageCount; pageNumber++)
        {
            GetMenuCardsForManageResponse page = await GetForRestaurantAsync<GetMenuCardsForManageResponse>(
                $"{BaseUrl}/manage?pageNumber={pageNumber}&pageSize={ManagementPageSize}",
                restaurantId,
                cancellationToken);
            items.AddRange(page.Items);
        }

        return firstPage with { Items = items };
    }

    private async Task<GetMenuCardForManageResponse> GetByIdForManageCoreAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await GetForRestaurantAsync<GetMenuCardForManageResponse>(
            $"{BaseUrl}/{menuCardId}/manage",
            restaurantId,
            cancellationToken);

    private async Task<TResponse> GetForRestaurantAsync<TResponse>(
        string uri,
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(HttpMethod.Get, uri, restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<TResponse> SendForRestaurantAsync<TResponse>(
        HttpMethod method,
        string uri,
        Guid restaurantId,
        object requestBody,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(method, uri, restaurantId);
        request.Content = JsonContent.Create(requestBody);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<bool> SendForRestaurantNoContentAsync(
        HttpMethod method,
        string uri,
        Guid restaurantId,
        object? requestBody,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(method, uri, restaurantId);
        if (requestBody is not null)
        {
            request.Content = JsonContent.Create(requestBody);
        }

        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
        return true;
    }
}
