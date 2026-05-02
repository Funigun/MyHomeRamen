using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    internal static HttpRequestMessage CreateGetMessage(string url)
    {
        return new(HttpMethod.Get, url);
    }

    internal static HttpRequestMessage CreatePostMessage(string url)
    {
        return new(HttpMethod.Post, url);
    }

    internal static HttpRequestMessage CreateDeleteMessage(string url)
    {
        return new(HttpMethod.Delete, url);
    }

    internal static HttpRequestMessage CreatePutMessage(string url)
    {
        return new(HttpMethod.Put, url);
    }

    internal static HttpRequestMessage AddAuthorizationHeader(this HttpRequestMessage requestMessage, UserRoles userRole, string userId = "")
    {
        (string token, string scheme) = userRole switch
        {
            UserRoles.Admin => (JwtTokenFactory.GenerateAdminToken(userId), ManagerScheme),
            UserRoles.Employee => (JwtTokenFactory.GenerateEmployeeToken(userId), EmployeeScheme),
            _ => (JwtTokenFactory.GenerateCustomerToken(userId), CustomerScheme)
        };

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Headers.Add(SchemeHeader, scheme);

        return requestMessage;
    }

    internal static HttpRequestMessage WithGuestCookie(this HttpRequestMessage requestMessage, string guestId)
    {
        requestMessage.Headers.Add("Cookie", $"guest_id={guestId}");
        return requestMessage;
    }

    internal static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage requestMessage, T body)
    {
        requestMessage.Content = JsonContent.Create(body);
        return requestMessage;
    }

    internal static async Task<TDto> ResponseToDto<TDto>(this HttpResponseMessage responseMessage)
    {
        string responseContent = await responseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return JsonSerializer.Deserialize<TDto>(responseContent, JsonOptions)!;
    }
}
