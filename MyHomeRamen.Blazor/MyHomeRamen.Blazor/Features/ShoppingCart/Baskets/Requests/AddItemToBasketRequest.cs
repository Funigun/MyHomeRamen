namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Requests;

public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<IngredientRequestDto> BaseIngredients,
    List<IngredientRequestDto> CustomIngredients,
    string? Comments);
