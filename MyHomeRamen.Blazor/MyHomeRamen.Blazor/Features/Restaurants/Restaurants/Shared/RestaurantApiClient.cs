using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed class RestaurantApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/restaurants/restaurants";
    private const string EmployeeOptionsUrl = "api/restaurants/employee-options";
    private const string CustomerOptionsUrl = "api/restaurants/options";
    private const string FailureMessage = "Restaurant request failed.";

    public Task<ApiResult<GetAvailableRestaurantsResponse>> GetAvailableAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetAvailableCoreAsync(cancellationToken));

    public Task<ApiResult<GetRestaurantByIdResponse>> GetByIdAsync(
        Guid restaurantId,
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetByIdCoreAsync(restaurantId, cancellationToken));

    public Task<ApiResult<GetEmployeeRestaurantOptionsResponse>> GetEmployeeOptionsAsync(
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetEmployeeOptionsCoreAsync(cancellationToken));

    public Task<ApiResult<GetCustomerRestaurantOptionsResponse>> GetCustomerOptionsAsync(
        CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetCustomerOptionsCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> CreateAsync(CreateRestaurantRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    private async Task<GetEmployeeRestaurantOptionsResponse> GetEmployeeOptionsCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(EmployeeOptionsUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetEmployeeRestaurantOptionsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetCustomerRestaurantOptionsResponse> GetCustomerOptionsCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(CustomerOptionsUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetCustomerRestaurantOptionsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetAvailableRestaurantsResponse> GetAvailableCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(BaseUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetAvailableRestaurantsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<GetRestaurantByIdResponse> GetByIdCoreAsync(
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/{restaurantId}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetRestaurantByIdResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task CreateCoreAsync(CreateRestaurantRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }
}
