using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public class ShoppingCartDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<ShoppingCartDbContext>
{
    public ShoppingCartDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ShoppingCartDbContext>? optionsBuilder = new DbContextOptionsBuilder<ShoppingCartDbContext>();
        optionsBuilder.UseSqlServer(DbConstants.MigrationConnectionString);

        return new ShoppingCartDbContext(optionsBuilder.Options, null!);
    }
}
