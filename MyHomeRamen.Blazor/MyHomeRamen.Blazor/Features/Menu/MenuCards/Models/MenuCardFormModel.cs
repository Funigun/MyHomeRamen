using MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

public sealed class MenuCardFormModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public CreateMenuCardRequest ToCreateRequest() => new(Name.Trim());

    public UpdateMenuCardRequest ToUpdateRequest() => new(Name.Trim(), IsActive);

    public static MenuCardFormModel FromResponse(GetMenuCardForManageResponse response) =>
        new() { Name = response.Name, IsActive = response.IsActive };
}
