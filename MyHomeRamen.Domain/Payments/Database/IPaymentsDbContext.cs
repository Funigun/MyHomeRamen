using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Domain.Payments.Database;

public interface IPaymentsDbContext : IBaseDbContext
{
    DbSet<PaymentMethod> PaymentMethods { get; }

    DbSet<PaymentChannel> PaymentChannels { get; }

    DbSet<PaymentGateway> PaymentGateways { get; }

    DbSet<Order> Orders { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }
}
