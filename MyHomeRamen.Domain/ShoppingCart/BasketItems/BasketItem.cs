using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Domain.ShoppingCart.BasketItems;

public sealed class BasketItem : AuditableEntity, IEntity<BasketItemId>
{
    public BasketItemId Id { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    public decimal Price { get; private set; }

    public string? Comment { get; private set; }

    private BasketItem()
    {
    }

    private BasketItem(BasketItemId id, Product product, int quantity, string? comment)
    {
        Id = id;
        Product = product;
        Quantity = quantity;
        Comment = comment;
    }

    public static BasketItem Create(BasketItemId id, Product product, int quantity, string? comment)
    {
        BasketItem item = new(id, product, quantity, comment);
        item.Price = item.Product.TotalPrice * item.Quantity;

        BasketItemValidator.Validate(item);
        return item;
    }
}
