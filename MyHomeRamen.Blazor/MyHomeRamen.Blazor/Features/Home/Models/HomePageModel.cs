namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class HomePageModel
{
    public BannerSectionModel Hero { get; init; } = new();

    public StorySectionModel Story { get; init; } = new();

    public MenuHighlightsSectionModel MenuHighlights { get; init; } = new();

    public SeasonalHighlightSectionModel SeasonalHighlight { get; init; } = new();

    public AtmosphereSectionModel Atmosphere { get; init; } = new();
}
