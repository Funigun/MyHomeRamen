using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.Payments;

namespace MyHomeRamen.Domain.Payments.Database;

public interface IPaymentsDbContext : IBaseDbContext
{
    DbSet<Payment> Payments { get; }

    DbSet<Order> Orders { get; }

    DbSet<User> Users { get; }
}
