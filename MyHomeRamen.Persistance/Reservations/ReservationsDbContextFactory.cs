using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Reservations;

public class ReservationsDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<ReservationsDbContext>
{
    public ReservationsDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                     .AddJsonFile("appsettings.json")
                                                     .Build();

        DbContextOptionsBuilder<ReservationsDbContext>? optionsBuilder = new DbContextOptionsBuilder<ReservationsDbContext>();
        optionsBuilder.UseSqlServer(configuration["ReservationsServiceDb"]);

        return new ReservationsDbContext(optionsBuilder.Options, null!);
    }
}
