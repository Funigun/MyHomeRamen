using System.Net.Http.Headers;
using System.Text.Json;

namespace MyHomeRamen.IntegrationTests.Common.Configuration;

internal static class HttpClientExtensions
{
    // Mirror scheme names from AuthorizationConfiguration in the API project
    private const string CustomerScheme = "RestaurantCustomer";
    private const string EmployeeScheme = "RestaurantEmployee";
    private const string ManagerScheme = "RestaurantManager";

    private const string SchemeHeader = "x-scheme";

    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal static void AddAuthorizationHeader(this HttpClient httpClient, UserRoles userRole)
    {
        (string token, string scheme) = userRole switch
        {
            UserRoles.Admin => (JwtTokenFactory.GenerateAdminToken(), ManagerScheme),
            UserRoles.Employee => (JwtTokenFactory.GenerateEmployeeToken(), EmployeeScheme),
            _ => (JwtTokenFactory.GenerateCustomerToken(), CustomerScheme)
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpClient.DefaultRequestHeaders.Remove(SchemeHeader);
        httpClient.DefaultRequestHeaders.Add(SchemeHeader, scheme);
    }

    internal static void ClearAuthorizationHeaders(this HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
        httpClient.DefaultRequestHeaders.Remove(SchemeHeader);
    }

    internal static async Task<TDto> ResponseToDto<TDto>(this HttpResponseMessage responseMessage)
    {
        string responseContent = await responseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return JsonSerializer.Deserialize<TDto>(responseContent, JsonOptions)!;
    }
}
