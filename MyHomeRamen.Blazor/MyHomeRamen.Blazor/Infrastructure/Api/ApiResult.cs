namespace MyHomeRamen.Blazor.Infrastructure.Api;

public abstract record ApiResult<T>
{
    private ApiResult()
    {
    }

    public sealed record Success(T Value) : ApiResult<T>;

    public sealed record Failure(ApiError Error) : ApiResult<T>;
}
