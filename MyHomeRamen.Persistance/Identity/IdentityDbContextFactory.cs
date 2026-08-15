using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Identity;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                             .AddJsonFile("appsettings.json")
                                             .AddUserSecrets<IdentityDbContext>(optional: true)
                                             .Build();

        DbContextOptionsBuilder<IdentityDbContext> optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(configuration["UserServiceDb"]);

        return new IdentityDbContext(optionsBuilder.Options, null!);
    }
}
