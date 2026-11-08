using MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Models;

public sealed class IngredientFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyCollection<Guid> CategoryIds { get; set; } = [];

    public CreateIngredientRequest ToCreateRequest() => new(Name, Description, CategoryIds);

    public UpdateIngredientRequest ToUpdateRequest() => new(Name, Description, CategoryIds);

    public static IngredientFormModel FromResponse(GetIngredientByIdResponse response) => new()
    {
        Name = response.Name,
        Description = response.Description,
        CategoryIds = response.CategoryIds.ToArray()
    };
}
