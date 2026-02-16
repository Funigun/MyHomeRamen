using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Domain.Menu.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Product> _favoriteProducts = [];

    public UserId Id { get; private set; }

    public IReadOnlyList<Product> FavoriteProducts => _favoriteProducts.ToList();

    private User()
    {
    }

    private User(UserId id, List<Product> favoriteProducts)
    {
        Id = id;
        _favoriteProducts = favoriteProducts;
    }
}
