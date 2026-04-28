using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.ShoppingCart;

public class ShoppingCartDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<ShoppingCartDbContext>
{
    public ShoppingCartDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                             .AddJsonFile("appsettings.json")
                                             .Build();

        DbContextOptionsBuilder<ShoppingCartDbContext>? optionsBuilder = new();
        optionsBuilder.UseSqlServer(configuration["ShoppingCartServiceDb"]);

        return new ShoppingCartDbContext(optionsBuilder.Options, null!);
    }
}
