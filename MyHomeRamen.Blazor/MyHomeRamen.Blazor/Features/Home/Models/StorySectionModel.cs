using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class StorySectionModel
{
    public ImageModel? Image { get; init; }

    public string Title { get; init; } = string.Empty;

    public List<string> Paragraphs { get; init; } = [];

    public List<StatHighlightModel> Stats { get; init; } = [];
}
