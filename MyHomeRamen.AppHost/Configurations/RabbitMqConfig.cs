namespace MyHomeRamen.AppHost.Configurations;

public sealed record RabbitMqConfig
{
    public string? BindMountFrom { get; init; }

    public string? BindMountTo { get; init; }
}
