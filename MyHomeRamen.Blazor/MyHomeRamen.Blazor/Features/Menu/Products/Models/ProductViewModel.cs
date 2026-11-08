using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.Common.Models;
using MyHomeRamen.Blazor.Features.Menu.Products.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Models;

public sealed record ProductViewModel(
    Guid Id,
    string Name,
    string Description,
    string Status,
    string CategoryName,
    IReadOnlyList<string> IngredientNames,
    IReadOnlyList<string> CustomIngredientNames,
    ImageModel Image)
{
    public static ProductViewModel FromResponse(
        ProductForManageResponse response,
        IReadOnlyList<CategoryOption> categories,
        IReadOnlyList<IngredientOption> ingredients,
        string imageAlt,
        string unavailableCategory,
        string unavailableIngredient) =>
        new(
            response.Id,
            response.Name,
            response.Description ?? string.Empty,
            response.Status,
            categories.FirstOrDefault(category => category.Id == response.CategoryId)?.Name ?? unavailableCategory,
            MapIngredientNames(response.IngredientIds, ingredients, unavailableIngredient),
            MapIngredientNames(response.CustomIngredientIds, ingredients, unavailableIngredient),
            new ImageModel { Alt = imageAlt });

    private static IReadOnlyList<string> MapIngredientNames(
        IEnumerable<Guid> ingredientIds,
        IReadOnlyList<IngredientOption> ingredients,
        string unavailableIngredient) =>
        ingredientIds
            .Select(id => ingredients.FirstOrDefault(ingredient => ingredient.Id == id)?.Name ?? unavailableIngredient)
            .ToArray();
}
