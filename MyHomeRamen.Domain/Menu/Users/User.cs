using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Roles;

namespace MyHomeRamen.Domain.Menu.Users;

public class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Product> _favoriteProducts = [];
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public IReadOnlyList<Product> FavoriteProducts => _favoriteProducts.ToList();

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    private User()
    {
    }

    private User(UserId id, List<Role> roles, List<Permission> permissions, List<Product> favoriteProducts)
    {
        Id = id;
        _roles = roles;
        _permissions = permissions;
        _favoriteProducts = favoriteProducts;
    }

    public static User Create(UserId id, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, roles, permissions, []);

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
