using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class BannerSectionModel
{
    public string Subtitle { get; init; } = string.Empty;

    public string HeadlineLine1 { get; init; } = string.Empty;

    public string HeadlineLine2 { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ImageModel? BackgroundImage { get; init; }
}
