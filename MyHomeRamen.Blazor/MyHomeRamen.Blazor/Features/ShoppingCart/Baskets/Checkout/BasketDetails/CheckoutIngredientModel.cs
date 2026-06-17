using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;

public class CheckoutIngredientModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public static CheckoutIngredientModel FromDetailsDto(BasketDetailsIngredientDto basketDetailsIngredientDto)
    {
        return new()
        {
            Id = basketDetailsIngredientDto.Id,
            Name = basketDetailsIngredientDto.Name,
            Description = basketDetailsIngredientDto.Description,
            Price = basketDetailsIngredientDto.Price
        };
    }
}
