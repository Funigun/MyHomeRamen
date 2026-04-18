namespace MyHomeRamen.Api.Common.Cache;

public interface ICachePolicy<TRequest, TCached>
{
    string GetKey(TRequest request);

    TimeSpan? ExpirationTime { get; }

    TimeSpan? LocalExpirationTime { get; }

    IEnumerable<string> Tags { get; }
}
