using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.ProductCustomization;

public sealed class ProductCustomizationModel
{
    public Guid ProductId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public string? Comments { get; set; }

    public List<IngredientCustomizationModel> BaseIngredients { get; init; } = [];

    public List<IngredientCustomizationModel> CustomIngredients { get; init; } = [];

    public static ProductCustomizationModel FromGetByIdResponse(GetProductByIdResponse response) =>
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

    public AddItemToBasketRequest ToAddItemToBasketRequest() =>
        new(
            ProductId,
            Quantity,
            BaseIngredients.Select(i => i.ToRequest()).ToList(),
            CustomIngredients.Where(i => i.IsSelected).Select(i => i.ToRequest()).ToList(),
            string.IsNullOrWhiteSpace(Comments) ? null : Comments);

    public ProductCustomizationModel FromBasketItemDetailsModel(CheckoutBasketItemModel checkoutItemModel) =>
        new()
        {
            ProductId = checkoutItemModel.Product.Id,
            Name = checkoutItemModel.Product.Name,
            Description = checkoutItemModel.Product.Description,
            Quantity = checkoutItemModel.Quantity,
            Comments = checkoutItemModel.Comment,
            BaseIngredients = checkoutItemModel.Product.BaseIngredients
                .Select(i => IngredientCustomizationModel.FromBasketDetailsDto(i))
                .ToList(),
            CustomIngredients = checkoutItemModel.Product.CustomIngredients
                .Select(i => IngredientCustomizationModel.FromBasketDetailsDto(i))
                .ToList()
        };
}
