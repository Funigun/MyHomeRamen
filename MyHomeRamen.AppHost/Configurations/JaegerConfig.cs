namespace MyHomeRamen.AppHost.Configurations;

public sealed record JaegerConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
