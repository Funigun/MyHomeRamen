namespace MyHomeRamen.Api.Common.Exceptions;

public sealed record ResponseBody
{
    public required int StatusCode { get; init; }

    public required string Message { get; init; }

    public IEnumerable<string> Errors { get; init; } = [];

    public Dictionary<string, IEnumerable<string>> ValidationErrors { get; init; } = [];
}
