using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Persistance.Identity;

namespace MyHomeRamen.Persistance.Users;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                             .AddJsonFile("appsettings.json")
                                             .Build();

        DbContextOptionsBuilder<IdentityDbContext> optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(configuration["UserCartServiceDb"]);

        return new IdentityDbContext(optionsBuilder.Options, new RestaurantConfigurationProvider(configuration), null!);
    }
}
