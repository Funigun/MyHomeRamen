using System.Net;

namespace MyHomeRamen.Blazor.Infrastructure.Api;

public static class ApiResultExtensions
{
    public static bool IsFailure<T>(this ApiResult<T> result) => result is ApiResult<T>.Failure;

    // Null for success and for errors already handled globally (redirects), so views show nothing extra.
    public static ApiError? GetDisplayableError<T>(this ApiResult<T> result) =>
        result is ApiResult<T>.Failure { Error.HandledGlobally: false } failure ? failure.Error : null;

    public static string ToMessage(this ApiError error, string badRequestMessage, string unexpectedMessage) =>
        error.GeneralError
        ?? (error.StatusCode == HttpStatusCode.BadRequest ? badRequestMessage : unexpectedMessage);
}
