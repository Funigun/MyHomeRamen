namespace MyHomeRamen.Blazor.Common.Models;

public sealed record PageState
{
    public int CurrentPage { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public int TotalCount { get; init; }

    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;

    public static PageState Default(int pageSize = 10) => new() { PageSize = pageSize };
}
