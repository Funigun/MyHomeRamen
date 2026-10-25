using MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Models;

public sealed record IngredientViewModel(Guid Id, string Name, string Description)
{
    public static IngredientViewModel FromResponse(IngredientForManageResponse response) =>
        new(response.Id, response.Name, response.Description);
}
