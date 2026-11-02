using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Payments.Features.Orders.Common;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Persistance.Payments.Converters;

namespace MyHomeRamen.Persistance.Payments;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options), IPaymentsDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;
    private ICompanyContext _companyContext = new CompanyContext();

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;

    }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options, currentUser)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public PaymentsDbContext(
        DbContextOptions<PaymentsDbContext> options,
        ICurrentUser currentUser,
        IServiceProvider serviceProvider,
        ICompanyContext companyContext) : this(options, currentUser, serviceProvider)
    {
        _companyContext = companyContext;
    }

    public Guid CompanyId => _companyContext.CompanyId
        ?? throw new InvalidOperationException("A valid restaurant context is required for Payments data access.");

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<PaymentChannel> PaymentChannels { get; set; }

    public DbSet<PaymentGateway> PaymentGateways { get; set; }

    public DbSet<Order> Orders { get; set; }

    public IPaymentMethodRepository PaymentMethod => _serviceProvider.GetService<IPaymentMethodRepository>() ?? throw new InvalidOperationException("PaymentMethodRepository is not registered in the service provider.");

    public IPaymentChannelRepository PaymentChannel => _serviceProvider.GetService<IPaymentChannelRepository>() ?? throw new InvalidOperationException("PaymentChannelRepository is not registered in the service provider.");

    public IPaymentGatewayRepository PaymentGateway => _serviceProvider.GetService<IPaymentGatewayRepository>() ?? throw new InvalidOperationException("PaymentGatewayRepository is not registered in the service provider.");

    public IOrderRepository Order => _serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("OrderRepository is not registered in the service provider.");

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateCompanyOwnership();
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateEntities()
    {
        DateTime currentDateTime = DateTime.UtcNow;

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.UserId.ToString();
                    entry.Entity.CreatedOn = currentDateTime;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.UserId.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }

    private void ValidateCompanyOwnership()
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            Guid? entityCompanyId = entry.Entity switch
            {
                PaymentMethod paymentMethod => paymentMethod.CompanyId,
                Order order => order.CompanyId,
                _ => null
            };

            if (entityCompanyId.HasValue && entityCompanyId != _companyContext.CompanyId)
            {
                throw new InvalidOperationException("Payments data must be written within its owning company context.");
            }
        }
    }

    public async Task<bool> EnsureCreated(CancellationToken cancellationToken)
    {
        IRelationalDatabaseCreator? dbCreator = Microsoft.EntityFrameworkCore.Infrastructure.AccessorExtensions.GetService<IRelationalDatabaseCreator>(Database);

        bool dbExists = dbCreator != null && await dbCreator.ExistsAsync(cancellationToken);

        if (!dbExists)
        {
            await dbCreator!.CreateAsync(cancellationToken);
        }

        return dbExists;
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        if ((await Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Payments.Configurations", StringComparison.OrdinalIgnoreCase));
        modelBuilder.Entity<PaymentMethod>().HasQueryFilter(entity =>
            _companyContext.CompanyId.HasValue && entity.CompanyId == _companyContext.CompanyId.Value);
        modelBuilder.Entity<Order>().HasQueryFilter(entity =>
            _companyContext.CompanyId.HasValue && entity.CompanyId == _companyContext.CompanyId.Value);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<PaymentMethodId>().HaveConversion<PaymentMethodIdConverter>();
        configurationBuilder.Properties<PaymentChannelId>().HaveConversion<PaymentChannelIdConverter>();
        configurationBuilder.Properties<PaymentGatewayId>().HaveConversion<PaymentGatewayIdConverter>();
        configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
    }
}
