using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class MenuItemModel
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Price { get; init; } = string.Empty;

    public ImageModel? Image { get; init; }

    public string? Badge { get; init; }

    public bool UseAltBackground { get; init; }

    public int ColSpan { get; init; } = 1;
}
