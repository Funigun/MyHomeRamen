using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Common.Extentsions;

public static class EndpointErrorExtensions
{
    /// <summary>
    /// Creates a successful <see cref="Result{TItem}"/> containing the specified item.
    /// </summary>
    /// <param name="item">The item to include in the successful result.</param>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <returns>An <see cref="Result{TItem}"/> representing a successful operation, with <see
    /// cref="Result{TItem}.Item"/> set to <paramref name="item"/> and <see
    /// cref="Result{TItem}.IsSuccessful"/> set to <see langword="true"/>.</returns>
    public static Result<TItem> Success<TItem>(this TItem item) =>
        new()
        {
            Item = item,
            IsSuccessful = true,
            Error = new Error(),
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result that represents a validation failure with the specified error message
    /// and validation errors.
    /// </summary>
    /// <param name="message">The error message that describes the validation failure. This message is intended to provide a general
    /// description of the validation error.</param>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <param name="validationErrors">A dictionary containing validation errors for specific fields, where each key is the field name and the value is
    /// a list of error messages associated with that field. Cannot be null.</param>
    /// <returns>An <see cref="Result{TItem}"/> instance indicating a validation failure, containing the provided error
    /// message and validation errors.</returns>
    public static Result<TItem> ValidationFailure<TItem>(string message, Dictionary<string, List<string>> validationErrors) =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.ValidationError,
                Message = message,
                ValidationErrors = validationErrors,
            },
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result indicating that the operation failed due to lack of authorization.
    /// </summary>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <returns>An <see cref="Result{TItem}"/> representing an authorization failure. The result's <c>IsSuccessful</c>
    /// property is <see langword="false"/>, and the error type is set to <see cref="ErrorType.Unauthorized"/>.</returns>
    public static Result<TItem> AuthorizationFailure<TItem>() =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.Unauthorized,
                Message = "You are not authorized to perform this action.",
            },
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result indicating that authentication is required.
    /// </summary>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <returns>An <see cref="Result{TItem}"/> representing a failed operation due to missing authentication. The
    /// result's <c>IsSuccessful</c> property is <see langword="false"/>, and the error type is set to <see
    /// cref="ErrorType.Forbidden"/>.</returns>
    public static Result<TItem> AuthenticationFailure<TItem>() =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.Forbidden,
                Message = "Sign in required.",
            },
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result indicating that the requested item was not found.
    /// </summary>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <param name="message">The error message describing the not found condition. Cannot be null.</param>
    /// <returns>An <see cref="Result{TItem}"/> representing a failed operation due to a not found error. The result's
    /// <c>IsSuccessful</c> property is <see langword="false"/>, and the <c>Error</c> property contains details about
    /// the not found error.</returns>
    public static Result<TItem> NotFoundFailure<TItem>(string message) =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.NotFound,
                Message = message,
            },
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result indicating that the requested resource is locked.
    /// </summary>
    /// <param name="message">The error message that describes the reason for the lock failure. Cannot be null.</param>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <returns>An <see cref="Result{TItem}"/> representing a failed operation due to a locked resource. The result's
    /// <c>IsSuccessful</c> property is <see langword="false"/>, and the <c>Error</c> property contains details about
    /// the lock failure.</returns>
    public static Result<TItem> LockFailure<TItem>(string message) =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.Locked,
                Message = message,
            },
        };

    /// <summary>
    /// Creates an unsuccessful endpoint result representing a general internal server error.
    /// </summary>
    /// <typeparam name="TItem">The second generic type parameter.</typeparam>
    /// <param name="message">The error message that describes the failure. This value is included in the result's error details.</param>
    /// <param name="errors">An optional list of additional error messages providing more details about the failure. Can be null if no
    /// additional details are available.</param>
    /// <returns>An <see cref="Result{TItem}"/> indicating failure, with error information set to represent an internal
    /// server error.</returns>
    public static Result<TItem> GeneralFailure<TItem>(string message, IEnumerable<string>? errors = null) =>
        new()
        {
            IsSuccessful = false,
            Error = new Error()
            {
                Type = ErrorType.InternalServerError,
                Message = message,
                Errors = errors?.ToList() ?? [],
            },
        };
}
