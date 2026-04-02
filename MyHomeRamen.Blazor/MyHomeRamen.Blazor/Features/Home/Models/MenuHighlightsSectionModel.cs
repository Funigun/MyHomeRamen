using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class MenuHighlightsSectionModel
{
    public string Title { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;

    public List<MenuItemModel> Items { get; init; } = [];
}
