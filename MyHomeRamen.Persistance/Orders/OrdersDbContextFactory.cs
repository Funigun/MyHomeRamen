using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Persistance.Orders;

public class OrdersDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                             .AddJsonFile("appsettings.json")
                                                             .Build();

        DbContextOptionsBuilder<OrdersDbContext>? optionsBuilder = new();
        optionsBuilder.UseSqlServer(configuration["OrdersServiceDb"]);

        return new OrdersDbContext(optionsBuilder.Options, null!);
    }
}
