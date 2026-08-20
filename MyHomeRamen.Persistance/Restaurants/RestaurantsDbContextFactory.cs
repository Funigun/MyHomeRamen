using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Restaurants;

public class RestaurantsDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<RestaurantsDbContext>
{
    public RestaurantsDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                     .AddJsonFile("appsettings.json")
                                                                     .Build();

        DbContextOptionsBuilder<RestaurantsDbContext>? optionsBuilder = new();
        optionsBuilder.UseSqlServer(configuration["RestaurantsServiceDb"]);

        return new RestaurantsDbContext(optionsBuilder.Options);
    }
}
