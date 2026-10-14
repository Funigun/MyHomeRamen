using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class CompanyApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string DetailsUrl = "api/restaurants/company/details";
    private const string RegisterOwnerUrl = "api/restaurants/company/register-owner";
    private const string FailureMessage = "Company request failed.";

    public Task<ApiResult<CompanyDetailsResponse>> GetDetailsAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetDetailsCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> UpdateDetailsAsync(CompanyDetailsUpdateRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateDetailsCoreAsync(request, cancellationToken));

    public Task<ApiResult<RegisterCompanyOwnerResponse>> RegisterCompanyOwnerAsync(RegisterCompanyOwnerRequest request, string idempotencyKey, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => RegisterCompanyOwnerCoreAsync(request, idempotencyKey, cancellationToken));

    private async Task<CompanyDetailsResponse> GetDetailsCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(DetailsUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<CompanyDetailsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task UpdateDetailsCoreAsync(CompanyDetailsUpdateRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync(DetailsUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task<RegisterCompanyOwnerResponse> RegisterCompanyOwnerCoreAsync(RegisterCompanyOwnerRequest request, string idempotencyKey, CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, RegisterOwnerUrl)
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add("X-Idempotency-Key", idempotencyKey);

        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<RegisterCompanyOwnerResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }
}
