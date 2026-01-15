namespace MyHomeRamen.Api.Common.Endpoint;

public sealed class Result<TItem>
{
    public TItem? Item { get; init; }

    public bool IsSuccessful { get; init; }

    public Error Error { get; init; } = new();

    public Result()
    {
    }
}
