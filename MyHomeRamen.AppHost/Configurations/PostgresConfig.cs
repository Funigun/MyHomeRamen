namespace MyHomeRamen.AppHost.Configurations;

public sealed record PostgresConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
