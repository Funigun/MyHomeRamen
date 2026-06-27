using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Domain.Menu.Database;

public interface IMenuDbContext : IBaseDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }
}
