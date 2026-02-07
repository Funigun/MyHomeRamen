namespace MyHomeRamen.AppHost.Configurations;

public sealed record SeqConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
