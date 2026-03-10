using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public class OrdersDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<OrdersDbContext>? optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseSqlServer(DbConstants.MigrationConnectionString);

        return new OrdersDbContext(optionsBuilder.Options, null!);
    }
}
