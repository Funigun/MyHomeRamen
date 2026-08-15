using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;

public class CheckoutProductModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public List<CheckoutIngredientModel> BaseIngredients { get; set; } = [];

    public List<CheckoutIngredientModel> CustomIngredients { get; set; } = [];

    public static CheckoutProductModel FromDetailsDto(BasketDetailsItemProductDto basketDetailsProductDto)
    {
        return new()
        {
            Id = basketDetailsProductDto.Id,
            Name = basketDetailsProductDto.Name,
            Description = basketDetailsProductDto.Description,
            ImageUrl = basketDetailsProductDto.ImageUrl,
            BaseIngredients = basketDetailsProductDto.BaseIngredients
                .Select(CheckoutIngredientModel.FromDetailsDto)
                .ToList(),
            CustomIngredients = basketDetailsProductDto.CustomIngredients
                .Select(CheckoutIngredientModel.FromDetailsDto)
                .ToList()
        };
    }
}
