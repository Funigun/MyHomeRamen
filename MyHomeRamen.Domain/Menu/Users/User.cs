using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Domain.Menu.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Product> _favoriteProducts = [];
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public IReadOnlyList<Product> FavoriteProducts => _favoriteProducts.ToList();

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    private User()
    {
    }

    private User(UserId id, Guid restaurantId, List<Role> roles, List<Permission> permissions, List<Product> favoriteProducts)
    {
        Id = id;
        RestaurantId = restaurantId;
        _roles = roles;
        _permissions = permissions;
        _favoriteProducts = favoriteProducts;
    }

    public static User Create(UserId id, Guid restaurantId, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, restaurantId, roles, permissions, []);

        UserValidator.Validate(user);

        return user;
    }

    public void AddFavoriteProduct(Product product)
    {
        if (!_favoriteProducts.Contains(product))
        {
            _favoriteProducts.Add(product);
        }
    }

    public void RemoveFavoriteProduct(Product product)
    {
        _favoriteProducts.Remove(product);
    }
}
