using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class CompanyApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string DetailsUrl = "api/restaurants/company/details";
    private const string SetupUrl = "api/restaurants/company/setup";
    private const string OwnerSetupUrl = $"{SetupUrl}/owner";
    private const string CompanySetupUrl = $"{SetupUrl}/company";
    private const string RestaurantSetupUrl = $"{SetupUrl}/restaurant";
    private const string SetupStatusUrl = $"{SetupUrl}/status";
    private const string FailureMessage = "Company request failed.";

    public Task<ApiResult<CompanySetupStatusResponse>> GetSetupStatusAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetSetupStatusCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> RegisterOwnerAsync(RegisterCompanyOwnerRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => RegisterOwnerCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> ConfigureCompanyAsync(ConfigureCompanyRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => ConfigureCompanyCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> CreateInitialRestaurantAsync(InitialRestaurantSetupRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateInitialRestaurantCoreAsync(request, cancellationToken));

    public Task<ApiResult<CompanyDetailsResponse>> GetDetailsAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetDetailsCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> UpdateDetailsAsync(CompanyDetailsUpdateRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateDetailsCoreAsync(request, cancellationToken));

    private async Task<CompanyDetailsResponse> GetDetailsCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(DetailsUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CompanyDetailsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<CompanySetupStatusResponse> GetSetupStatusCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(SetupStatusUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CompanySetupStatusResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task RegisterOwnerCoreAsync(RegisterCompanyOwnerRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(OwnerSetupUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task ConfigureCompanyCoreAsync(ConfigureCompanyRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(CompanySetupUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task CreateInitialRestaurantCoreAsync(InitialRestaurantSetupRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(RestaurantSetupUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task UpdateDetailsCoreAsync(CompanyDetailsUpdateRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync(DetailsUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

}
