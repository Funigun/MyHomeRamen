using MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

public sealed class MenuSectionFormModel
{
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public CreateMenuSectionRequest ToCreateRequest() => new(Name.Trim(), SortOrder);

    public UpdateMenuSectionRequest ToUpdateRequest() => new(Name.Trim(), SortOrder);
}
