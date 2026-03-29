namespace MyHomeRamen.AppHost.Configurations.Common;

public abstract record BaseConfiguration
{
    public string UserName { get; init; } = "admin";

    public string Password { get; init; } = "admin";

    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
