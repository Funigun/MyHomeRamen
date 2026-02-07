namespace MyHomeRamen.AppHost.Configurations;

public sealed record KeyCloakConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
