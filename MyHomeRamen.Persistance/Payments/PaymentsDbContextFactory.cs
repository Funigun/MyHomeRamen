using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public class PaymentsDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<PaymentsDbContext>? optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseSqlServer(DbConstants.MigrationConnectionString);

        return new PaymentsDbContext(optionsBuilder.Options, null!);
    }
}
