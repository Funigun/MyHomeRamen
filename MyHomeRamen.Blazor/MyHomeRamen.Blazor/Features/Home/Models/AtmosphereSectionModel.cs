using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Home.Models;

public sealed class AtmosphereSectionModel
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ImageModel? AtmosphereImage { get; init; }
}
