using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Features.Common.Configurations;

namespace MyHomeRamen.Persistance.Users;

public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                             .AddJsonFile("appsettings.json")
                                             .Build();

        DbContextOptionsBuilder<UsersDbContext> optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder.UseSqlServer(configuration["UserCartServiceDb"]);

        return new UsersDbContext(optionsBuilder.Options, new RestaurantConfigurationProvider(configuration), null!);
    }
}
