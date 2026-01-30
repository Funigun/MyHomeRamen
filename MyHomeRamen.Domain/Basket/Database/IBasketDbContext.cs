using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket.Database;

public interface IBasketDbContext : IBaseDbContext
{
    DbSet<ShoppingCart> ShoppingCarts { get; }

    DbSet<Product> Products { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
