using MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientTableModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public static IngredientTableModel FromResponse(IngredientForManageItemResponse response)
    {
        return new IngredientTableModel
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description,
        };
    }
}
