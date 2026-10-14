using System.Net;
using System.Text.Json;

namespace MyHomeRamen.Blazor.Infrastructure.Api;

public sealed class ApiRequestException(
    HttpStatusCode statusCode,
    string message,
    IReadOnlyDictionary<string, string[]>? validationErrors = null,
    string? generalError = null) : ApiException(statusCode, message, validationErrors, generalError)
{
    public static async Task<ApiRequestException> FromResponseAsync(
        HttpResponseMessage response,
        string fallbackMessage,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            return new ApiRequestException(response.StatusCode, fallbackMessage);
        }

        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return new ApiRequestException(response.StatusCode, fallbackMessage);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(body);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return new ApiRequestException(response.StatusCode, fallbackMessage);
            }

            Dictionary<string, string[]> errors = ReadValidationErrors(root);
            string? generalError = ReadString(root, "detail") ?? ReadString(root, "title");

            return new ApiRequestException(response.StatusCode, generalError ?? fallbackMessage, errors, generalError);
        }
        catch (JsonException)
        {
            return new ApiRequestException(response.StatusCode, fallbackMessage);
        }
    }

    private static Dictionary<string, string[]> ReadValidationErrors(JsonElement root)
    {
        Dictionary<string, string[]> errors = new(StringComparer.OrdinalIgnoreCase);
        if (!root.TryGetProperty("errors", out JsonElement errorsElement) || errorsElement.ValueKind != JsonValueKind.Object)
        {
            return errors;
        }

        foreach (JsonProperty error in errorsElement.EnumerateObject())
        {
            errors[error.Name] = error.Value.ValueKind == JsonValueKind.Array
                ? error.Value.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.String).Select(item => item.GetString()!).ToArray()
                : [error.Value.GetString() ?? string.Empty];
        }

        return errors;
    }

    private static string? ReadString(JsonElement root, string propertyName) =>
        root.TryGetProperty(propertyName, out JsonElement element) && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : null;
}
