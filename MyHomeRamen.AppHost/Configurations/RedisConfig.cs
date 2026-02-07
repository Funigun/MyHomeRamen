namespace MyHomeRamen.AppHost.Configurations;

public sealed record RedisConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
