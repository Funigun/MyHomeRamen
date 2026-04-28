using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Payments;

public class PaymentsDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                     .AddJsonFile("appsettings.json")
                                                     .Build();

        DbContextOptionsBuilder<PaymentsDbContext>? optionsBuilder = new();
        optionsBuilder.UseSqlServer(configuration["PaymentServiceDb"]);

        return new PaymentsDbContext(optionsBuilder.Options, null!);
    }
}
