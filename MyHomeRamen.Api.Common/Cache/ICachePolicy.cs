namespace MyHomeRamen.Api.Common.Cache;

public interface ICachePolicy<TRequest, TResponse>
{
    string Key { get; }

    TimeSpan? ExpirationTime { get; }

    TimeSpan? LocalExpirationTime { get; }
}
