using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public class MenuDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<MenuDbContext>
{
    public MenuDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MenuDbContext>? optionsBuilder = new DbContextOptionsBuilder<MenuDbContext>();
        optionsBuilder.UseSqlServer(DbConstants.MigrationConnectionString);

        return new MenuDbContext(optionsBuilder.Options, null!);
    }
}
