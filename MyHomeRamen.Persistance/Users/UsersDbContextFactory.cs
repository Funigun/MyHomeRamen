using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Api.Common.Configuration;

namespace MyHomeRamen.Persistance.Users;

public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        DbContextOptionsBuilder<UsersDbContext> optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=MyHomeRamenDb;Trusted_Connection=True;TrustServerCertificate=True");

        return new UsersDbContext(optionsBuilder.Options, new RestaurantConfigurationProvider(configuration), null!);
    }
}
