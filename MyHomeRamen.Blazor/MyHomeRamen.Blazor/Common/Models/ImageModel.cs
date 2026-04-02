namespace MyHomeRamen.Blazor.Common.Models;

public sealed class ImageModel
{
    public string? Url { get; init; }

    public string Alt { get; init; } = string.Empty;
}
