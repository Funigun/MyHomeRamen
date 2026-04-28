using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Menu;

public class MenuDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<MenuDbContext>
{
    public MenuDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                     .AddJsonFile("appsettings.json")
                                                                     .Build();

        DbContextOptionsBuilder<MenuDbContext>? optionsBuilder = new();
        optionsBuilder.UseSqlServer(configuration["MenuServiceDb"]);

        return new MenuDbContext(optionsBuilder.Options, null!);
    }
}
