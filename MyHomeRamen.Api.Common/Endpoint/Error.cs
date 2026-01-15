namespace MyHomeRamen.Api.Common.Endpoint;

public sealed class Error
{
    public ErrorType Type { get; init; }

    public string Message { get; init; } = string.Empty;

    public ICollection<string> Errors { get; init; } = [];

    public Dictionary<string, List<string>> ValidationErrors { get; init; } = [];
}
