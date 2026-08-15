using MyHomeRamen.Blazor.Features.Menu.Common.Models;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Responses;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public IEnumerable<CategoryOption> CategoryIds { get; set; } = [];

    public CreateIngredientRequest ToCreateRequest()
    {
        return new CreateIngredientRequest(Name, Description, Price, CategoryIds.Select(c => c.Id));
    }

    public UpdateIngredientRequest ToEditRequest()
    {
        return new UpdateIngredientRequest(Name, Description, Price, CategoryIds.Select(c => c.Id));
    }

    public static IngredientModel FromResponse(GetIngredientByIdResponse response, IEnumerable<CategoryOption> availableCategories)
    {
        return new IngredientModel
        {
            Name = response.Name,
            Description = response.Description,
            Price = response.Price,
            CategoryIds = availableCategories.Where(c => response.CategoryIds.Contains(c.Id)),
        };
    }
}
