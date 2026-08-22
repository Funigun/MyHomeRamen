using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Features.Orders.Features.Ingredients.Common;
using MyHomeRamen.Features.Orders.Features.Orders.Common;
using MyHomeRamen.Features.Orders.Features.Payments.Common;
using MyHomeRamen.Features.Orders.Features.Products.Common;

using MyHomeRamen.Persistance.Orders.Converters;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options), IOrdersDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public IOrderRepository Order => _serviceProvider.GetService<IOrderRepository>() ?? throw new InvalidOperationException("OrderRepository is not registered in the service provider.");
    public IProductRepository Product => _serviceProvider.GetService<IProductRepository>() ?? throw new InvalidOperationException("ProductRepository is not registered in the service provider.");
    public IIngredientRepository Ingredient => _serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("IngredientRepository is not registered in the service provider.");
    public IPaymentRepository Payment => _serviceProvider.GetService<IPaymentRepository>() ?? throw new InvalidOperationException("PaymentRepository is not registered in the service provider.");

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
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

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        if ((await Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }

    public async Task Seed(CancellationToken cancellationToken)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Orders.Configurations", StringComparison.OrdinalIgnoreCase));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<PaymentId>().HaveConversion<PaymentIdConverter>();
    }
}
