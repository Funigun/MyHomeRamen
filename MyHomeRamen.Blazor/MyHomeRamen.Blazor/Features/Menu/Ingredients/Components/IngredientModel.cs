using MyHomeRamen.Blazor.Features.Menu.Ingredients.Requests;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

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

    // TODO: Map to EditIngredientRequest once EditIngredient feature is implemented
    public CreateIngredientRequest ToEditRequest()
    {
        return new CreateIngredientRequest(Name, Description, Price, CategoryIds);
    }

    public static IngredientModel FromResponse(GetIngredientByIdResponse response)
    {
        return new IngredientModel
        {
            Name = response.Name,
            Description = response.Description,
            Price = response.Price,
            CategoryIds = response.CategoryIds,
        };
    }
}
