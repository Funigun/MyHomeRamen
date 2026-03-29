using MyHomeRamen.Blazor.Features.Menu.Ingredients.CreateIngredient;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public IEnumerable<Guid> CategoryIds { get; set; } = [];

    public CreateIngredientRequest ToCreateRequest()
    {
        return new CreateIngredientRequest(Name, Description, Price, CategoryIds);
    }
}
