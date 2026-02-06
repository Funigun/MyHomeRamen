using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Domain.ShoppingCart.Database;

public interface IShoppingCartDbContext : IBaseDbContext
{
    DbSet<Basket> ShoppingCarts { get; }

    DbSet<Product> Products { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
