using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Models;

public sealed class ProductCustomizationModel
{
    public Guid ProductId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public string? Comments { get; set; }

    public List<IngredientCustomizationModel> BaseIngredients { get; init; } = [];

    public List<IngredientCustomizationModel> CustomIngredients { get; init; } = [];

    public static ProductCustomizationModel FromResponse(GetProductByIdResponse response) =>
        new()
        {
            ProductId = response.Id,
            Name = response.Name,
            Description = response.Description,
            Quantity = 1,
            BaseIngredients = response.BaseIngredients
                .Select(i => IngredientCustomizationModel.FromDto(i, selectedByDefault: true))
                .ToList(),
            CustomIngredients = response.CustomIngredients
                .Select(i => IngredientCustomizationModel.FromDto(i, selectedByDefault: true))
                .ToList()
        };

    public AddItemToBasketRequest ToRequest() =>
        new(
            ProductId,
            Quantity,
            BaseIngredients.Select(i => i.ToRequest()).ToList(),
            CustomIngredients.Where(i => i.IsSelected).Select(i => i.ToRequest()).ToList(),
            string.IsNullOrWhiteSpace(Comments) ? null : Comments);
}
