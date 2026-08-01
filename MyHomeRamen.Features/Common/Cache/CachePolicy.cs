namespace MyHomeRamen.Features.Common.Cache;

public sealed record LocalCachePolicy(string ModuleName, string Key, TimeSpan LocalExpirationTime, IEnumerable<string> Tags);

public sealed record DistributedCachePolicy(string ModuleName, string Key, TimeSpan ExpirationTime, IEnumerable<string> Tags);

public sealed record HybridCachePolicy(string ModuleName, string Key, TimeSpan ExpirationTime, TimeSpan LocalExpirationTime, IEnumerable<string> Tags);

public sealed record CachePolicy
{
    private LocalCachePolicy? Local { get; init; }

    private DistributedCachePolicy? Distributed { get; init; }

    private HybridCachePolicy? Hybrid { get; init; }

    public string Module => Local?.ModuleName ?? Distributed?.ModuleName ?? Hybrid?.ModuleName ?? throw new InvalidOperationException("Cache policy is not set.");

    public string Key => Local?.Key ?? Distributed?.Key ?? Hybrid?.Key ?? throw new InvalidOperationException("Cache policy is not set.");

    public TimeSpan? DistributedExpirationTime => Distributed?.ExpirationTime ?? Hybrid?.ExpirationTime ?? null;

    public TimeSpan? LocalExpirationTime => Local?.LocalExpirationTime ?? Hybrid?.LocalExpirationTime ?? null;

    public IEnumerable<string> Tags => Local?.Tags ?? Distributed?.Tags ?? Hybrid?.Tags ?? [];

    public static CachePolicy LocalCache<TModule>(string key, TimeSpan localExpirationTime, IEnumerable<string> tags)  
           where TModule : ICacheModule
     => new() { Local = new(TModule.ModuleName, key, localExpirationTime, tags) }; 

    public static CachePolicy DistributedCache<TModule>(string key, TimeSpan expirationTime, IEnumerable<string> tags)
           where TModule : ICacheModule
        => new() { Distributed = new(TModule.ModuleName, key, expirationTime, tags) };

    public static CachePolicy HybridCache<TModule>(string key, TimeSpan expirationTime, TimeSpan localExpirationTime, IEnumerable<string> tags)
           where TModule : ICacheModule
        => new() { Hybrid = new(TModule.ModuleName, key, expirationTime, localExpirationTime, tags) };
}
