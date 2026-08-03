using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

internal static class Mappings
{
    internal static Product ToShoppingCartProduct(
        this MenuProductResult result,
        IEnumerable<BasketIngredientDto> baseIngredients,
        IEnumerable<BasketIngredientDto> customIngredients)
    {
        List<Ingredient> base_ = result.BaseIngredients
            .Select(i =>
            {
                int qty = baseIngredients.FirstOrDefault(r => r.Id == i.Id)?.Quantity ?? 1;
                return Ingredient.Create(
                    new IngredientId(Guid.CreateVersion7()),
                    new IngredientId(i.Id),
                    i.Name,
                    i.Description,
                    i.Price,
                    qty);
            })
            .ToList();

        List<Ingredient> custom = result.CustomIngredients
            .Select(i =>
            {
                int qty = customIngredients.FirstOrDefault(r => r.Id == i.Id)?.Quantity ?? 1;
                return Ingredient.Create(
                    new IngredientId(Guid.CreateVersion7()),
                    new IngredientId(i.Id),
                    i.Name,
                    i.Description,
                    i.Price,
                    qty);
            })
            .ToList();

        return Product.Create(
            new ProductId(Guid.CreateVersion7()),
            new ProductId(result.Id),
            result.Name,
            result.Description,
            result.Price,
            result.ImageUrl,
            base_,
            custom);
    }

    internal static BasketItem ToBasketItem(
        this Product product,
        int quantity,
        string? comment)
    {
        return BasketItem.Create(
            new BasketItemId(Guid.CreateVersion7()),
            product,
            quantity,
            comment);
    }
}

