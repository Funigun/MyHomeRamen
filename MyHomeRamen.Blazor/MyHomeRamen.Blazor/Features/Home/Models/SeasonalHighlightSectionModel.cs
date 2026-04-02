using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class SeasonalHighlightSectionModel
{
    public string Label { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string LinkText { get; init; } = string.Empty;

    public string LinkHref { get; init; } = string.Empty;

    public ImageModel? Image { get; init; }
}
