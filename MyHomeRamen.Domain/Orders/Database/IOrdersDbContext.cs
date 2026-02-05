using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Products;

namespace MyHomeRamen.Domain.Orders.Database;

public interface IOrdersDbContext : IBaseDbContext
{
    DbSet<Order> Orders { get; }

    DbSet<Product> Products { get; }

    DbSet<Ingredient> Ingredients { get; }

    DbSet<User> Users { get; }
}
