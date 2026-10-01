using System.Net;
using System.Text.Json;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class CompanyApiClient(HttpClient httpClient)
{
    private const string DetailsUrl = "api/restaurants/company/details";

    public async Task<CompanyDetailsDto> GetDetailsAsync(CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(DetailsUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CompanyApiException.FromResponseAsync(response, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<CompanyDetailsDto>(cancellationToken: cancellationToken)
            ?? throw new CompanyApiException(HttpStatusCode.InternalServerError, "Company details response was empty.");
    }

    public async Task UpdateDetailsAsync(UpdateCompanyDetailsRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync(DetailsUrl, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CompanyApiException.FromResponseAsync(response, cancellationToken);
        }
    }

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

        RegisterCompanyOwnerResponse? result = await response.Content.ReadFromJsonAsync<RegisterCompanyOwnerResponse>(cancellationToken: cancellationToken);
        return result ?? throw new RegistrationApiException(HttpStatusCode.InternalServerError);
    }
}

public sealed class CompanyApiException(HttpStatusCode statusCode, string message, IReadOnlyDictionary<string, string[]>? validationErrors = null, string? generalError = null) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; } = validationErrors ?? new Dictionary<string, string[]>();
    public string? GeneralError { get; } = generalError;

    public static async Task<CompanyApiException> FromResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(body);
                JsonElement root = document.RootElement;
                Dictionary<string, string[]> errors = new(StringComparer.OrdinalIgnoreCase);
                string? generalError = null;

                if (root.TryGetProperty("errors", out JsonElement errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty error in errorsElement.EnumerateObject())
                    {
                        errors[error.Name] = error.Value.ValueKind == JsonValueKind.Array
                            ? error.Value.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.String).Select(item => item.GetString()!).ToArray()
                            : [error.Value.GetString() ?? string.Empty];
                    }
                }

                if (root.TryGetProperty("detail", out JsonElement detail) && detail.ValueKind == JsonValueKind.String)
                {
                    generalError = detail.GetString();
                }
                else if (root.TryGetProperty("title", out JsonElement title) && title.ValueKind == JsonValueKind.String)
                {
                    generalError = title.GetString();
                }

                return new CompanyApiException(response.StatusCode, generalError ?? "Company details request was rejected.", errors, generalError);
            }
            catch (JsonException)
            {
            }
        }

        string message = response.StatusCode switch
        {
            HttpStatusCode.Forbidden => "You are not authorized to manage company details.",
            _ => "Company details request failed. Please try again."
        };
        return new CompanyApiException(response.StatusCode, message);
    }
}

public sealed class RegistrationApiException(HttpStatusCode statusCode) : Exception
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
