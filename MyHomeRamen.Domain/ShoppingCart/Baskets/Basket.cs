using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

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
        Basket basket = new(id, user);

        BasketValidator.Validate(basket);

        return basket;
    }
}
