using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart;

public sealed class Basket : AuditableEntity, IEntity<BasketId>
{
    private readonly List<Product> _products = [];

    public BasketId Id { get; private set; }

    public User User { get; private set; }

    public IReadOnlyList<Product> Products => _products.ToList();

    private Basket()
    {
    }

    private Basket(BasketId id, User user)
    {
        Id = id;
        User = user;
    }

    public static Basket Create(BasketId id, User user)
    {
        return new Basket(id, user);
    }
}
