using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Database;

public interface IShoppingCartDbContext : IBaseDbContext
{
    DbSet<Basket> ShoppingCarts { get; }

    DbSet<Product> Products { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
