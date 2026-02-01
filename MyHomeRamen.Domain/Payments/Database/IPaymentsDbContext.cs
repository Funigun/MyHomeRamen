using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.Database;

public interface IPaymentsDbContext : IBaseDbContext
{
    DbSet<Payment> Payments { get; }

    DbSet<Order> Orders { get; }

    DbSet<User> Users { get; }
}
