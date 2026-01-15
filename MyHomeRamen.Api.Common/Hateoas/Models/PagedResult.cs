namespace MyHomeRamen.Api.Common.Hateoas.Models;

public abstract record PagedResult
{
    public int Page { get; protected set; }

    public int PageSize { get; protected set; }

    public int TotalCount { get; protected set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected PagedResult(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
