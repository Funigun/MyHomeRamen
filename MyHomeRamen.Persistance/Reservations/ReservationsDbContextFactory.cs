using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public class ReservationsDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<ReservationsDbContext>
{
    public ReservationsDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ReservationsDbContext>? optionsBuilder = new DbContextOptionsBuilder<ReservationsDbContext>();
        optionsBuilder.UseSqlServer(DbConstants.MigrationConnectionString);

        return new ReservationsDbContext(optionsBuilder.Options, null!);
    }
}
