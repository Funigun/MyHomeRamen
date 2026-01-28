using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public sealed class ShoppingCart : AuditableEntity, IEntity<ShoppingCartId>
{
    private readonly List<Product> _products = new();

    public ShoppingCartId Id { get; private set; }

    public User User { get; private set; }

    public IReadOnlyList<Product> Products => _products.ToList();

    private ShoppingCart()
    {
    }

    private ShoppingCart(ShoppingCartId id, User user)
    {
        Id = id;
        User = user;
    }

    public static ShoppingCart Create(ShoppingCartId id, User user)
    {
        return new ShoppingCart(id, user);
    }
}
