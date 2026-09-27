using System.Net;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class CompanyApiClient(HttpClient httpClient)
{
    private const string BASE_URL = "api/companies";

    public async Task<RegisterCompanyOwnerResponse> RegisterCompanyOwnerAsync(RegisterCompanyOwnerRequest request, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, "/api/restaurants/company/register-owner")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add("X-Idempotency-Key", idempotencyKey);

        using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new RegistrationApiException(response.StatusCode);
        }

        RegisterCompanyOwnerResponse? result = await response.Content.ReadFromJsonAsync<RegisterCompanyOwnerResponse>(
            cancellationToken: cancellationToken);

        return result ?? throw new RegistrationApiException(HttpStatusCode.InternalServerError);
    }

    public async Task<CompanyDetailsDto> GetDetails(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response = await httpClient.GetAsync($"{BASE_URL}/details", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CompanyDetailsDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Unable to deserialize CompanyDetailsDto from response.");
    }
}

public sealed class RegistrationApiException(HttpStatusCode statusCode) : Exception
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
