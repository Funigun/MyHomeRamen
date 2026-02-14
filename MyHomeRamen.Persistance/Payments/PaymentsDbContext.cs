using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Payments;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentGroups;
using MyHomeRamen.Domain.Payments.PaymentProviders;
using MyHomeRamen.Domain.Payments.Payments;
using MyHomeRamen.Persistance.Payments.Converters;

namespace MyHomeRamen.Persistance.Payments;

public class PaymentsDbContext : DbContext, IPaymentsDbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<PaymentProvider> PaymentProviders { get; set; }

    public DbSet<PaymentGroup> PaymentGroups { get; set; }

    public Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransaction(CancellationToken cancellationToken)
    {
        return Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransaction(CancellationToken cancellationToken)
    {
        return Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<bool> EnsureCreated(CancellationToken cancellationToken)
    {
        return await Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        await Database.MigrateAsync(cancellationToken);
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlAsync(sql, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Payments.Configurations"));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<PaymentId>().HaveConversion<PaymentIdConverter>();
        configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<PaymentProviderId>().HaveConversion<PaymentProviderIdConverter>();
        configurationBuilder.Properties<PaymentGroupId>().HaveConversion<PaymentGroupIdConverter>();
    }
}
