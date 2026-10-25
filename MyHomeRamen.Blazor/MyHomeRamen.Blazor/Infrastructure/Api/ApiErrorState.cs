namespace MyHomeRamen.Blazor.Infrastructure.Api;

public sealed class ApiErrorState
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors = new Dictionary<string, string[]>();

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; private set; } = NoErrors;

    public string? Message { get; private set; }

    public bool HasError => Message is not null;

    public void Clear()
    {
        ValidationErrors = NoErrors;
        Message = null;
    }

    // Returns true when result is a failure, including errors handled globally (no message shown).
    public bool Apply<T>(ApiResult<T> result, string badRequestMessage, string unexpectedMessage)
    {
        Clear();

        ApiError? error = result.GetDisplayableError();
        if (error is not null)
        {
            ValidationErrors = error.ValidationErrors;
            Message = error.ToMessage(badRequestMessage, unexpectedMessage);
        }

        return result.IsFailure();
    }
}
