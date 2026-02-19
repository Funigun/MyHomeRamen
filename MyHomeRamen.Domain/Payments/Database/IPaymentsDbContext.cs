using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentGroups;
using MyHomeRamen.Domain.Payments.PaymentProviders;
using MyHomeRamen.Domain.Payments.Payments;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Domain.Payments.Database;

public interface IPaymentsDbContext : IBaseDbContext
{
    DbSet<Payment> Payments { get; }

    DbSet<Order> Orders { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<PaymentProvider> PaymentProviders { get; }

    DbSet<PaymentGroup> PaymentGroups { get; }
}
