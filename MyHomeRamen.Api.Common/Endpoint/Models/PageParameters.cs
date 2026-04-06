namespace MyHomeRamen.Api.Common.Endpoint.Models;

public sealed record PageParameters
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
