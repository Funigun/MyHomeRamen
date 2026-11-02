using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Domain.ShoppingCart.BasketItems;

public sealed class BasketItem : AuditableEntity, IEntity<BasketItemId>
{
    public BasketItemId Id { get; private set; }

    public Guid RestaurantProductOfferId { get; private set; }

    public Product Product { get; private set; } = null!;

    public IReadOnlyList<IngredientId> SelectedExtraIngredientIds =>
        Product.CustomIngredients.Select(ingredient => ingredient.OriginalId).ToList();

    public int Quantity { get; private set; }

    public decimal Price { get; private set; }

    public string? Comment { get; private set; }

    private BasketItem()
    {
    }

    private BasketItem(BasketItemId id, Guid restaurantProductOfferId, Product product, int quantity, string? comment)
    {
        Id = id;
        RestaurantProductOfferId = restaurantProductOfferId;
        Product = product;
        Quantity = quantity;
        Comment = comment;
    }

    public static BasketItem Create(BasketItemId id, Guid restaurantProductOfferId, Product product, int quantity, string? comment)
    {
        BasketItem item = new(id, restaurantProductOfferId, product, quantity, comment);
        item.Price = item.Product.TotalPrice * item.Quantity;

        BasketItemValidator.Validate(item);
        return item;
    }

    public void UpdateQuantity(int quantity)
    {
        BasketItem updated = Create(Id, RestaurantProductOfferId, Product, quantity, Comment);
        Quantity = updated.Quantity;
        Price = updated.Price;
    }
}
