using System.Net;

namespace MyHomeRamen.Blazor.Infrastructure.Api;

public abstract class ApiException(HttpStatusCode statusCode, string message, IReadOnlyDictionary<string, string[]>? validationErrors = null, string? generalError = null) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; } = validationErrors ?? new Dictionary<string, string[]>();
    public string? GeneralError { get; } = generalError;
}
