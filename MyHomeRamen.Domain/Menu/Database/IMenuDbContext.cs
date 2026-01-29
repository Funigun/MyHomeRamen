using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu.Database;

public interface IMenuDbContext : IBaseDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
