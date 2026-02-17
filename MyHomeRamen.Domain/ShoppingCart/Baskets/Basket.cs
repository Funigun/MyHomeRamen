using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

public sealed class Basket : AuditableEntity, IEntity<BasketId>
{
    private readonly List<Product> _products = [];

    public BasketId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public User User { get; private set; }

    public IReadOnlyList<Product> Products => _products.ToList();

    private Basket()
    {
    }

    private Basket(BasketId id, Guid restaurantId, User user)
    {
        Id = id;
        RestaurantId = restaurantId;
        User = user;
    }

    public static Basket Create(BasketId id, Guid restaurantId, User user)
    {
        Basket basket = new(id, restaurantId, user);

        BasketValidator.Validate(basket);

        return basket;
    }
}
