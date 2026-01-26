using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Product> _favoriteProducts = [];

    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public IReadOnlyList<Product> FavoriteProducts => _favoriteProducts.ToList();

    private User()
    {
    }

    private User(UserId id, List<Product> favoriteProducts)
    {
        Id = id;
        _favoriteProducts = favoriteProducts;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, List<Product> favoriteProducts)
    {
        return new User(id, favoriteProducts)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
}
