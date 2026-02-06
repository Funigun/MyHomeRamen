using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentGroups;
using MyHomeRamen.Domain.Payments.PaymentProviders;
using MyHomeRamen.Domain.Payments.Payments;

namespace MyHomeRamen.Domain.Payments.Database;

public interface IPaymentsDbContext : IBaseDbContext
{
    DbSet<Payment> Payments { get; }

    DbSet<Order> Orders { get; }

    DbSet<User> Users { get; }

    DbSet<PaymentProvider> PaymentProviders { get; }

    DbSet<PaymentGroup> PaymentGroups { get; }
}
