using System.Net;

namespace MyHomeRamen.Blazor.Infrastructure.Api;

public sealed record ApiError(
    HttpStatusCode? StatusCode,
    string Message,
    IReadOnlyDictionary<string, string[]> ValidationErrors,
    string? GeneralError,
    bool HandledGlobally,
    bool IsNetworkError = false);
