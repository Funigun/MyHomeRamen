using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Domain.Menu.Database;

public interface IMenuDbContext : IBaseDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
