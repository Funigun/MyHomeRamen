namespace MyHomeRamen.Api.Common.Hateoas.Models;

public abstract record PagedParameters
{
    public int Page { get; init; }

    public int PageSize { get; init; }

    protected PagedParameters(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
