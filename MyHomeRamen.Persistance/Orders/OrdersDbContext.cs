using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Orders.Database;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Persistance.Orders.Converters;

namespace MyHomeRamen.Persistance.Orders;

public class OrdersDbContext : DbContext, IOrdersDbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

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

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlAsync(sql, cancellationToken);
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        if ((await Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }

    public async Task Seed(Guid restaurantId, CancellationToken cancellationToken)
    {
        IEnumerable<string> roles = RoleConstants.AvailableRoles;
        IEnumerable<string> permissions = PermissionConstants.AvailablePermissions;

        HashSet<string> existingRoles = await Roles.AsNoTracking().Select(role => role.Name).ToHashSetAsync(cancellationToken);
        HashSet<string> existingPermissions = await Permissions.AsNoTracking().Select(permission => permission.Name).ToHashSetAsync(cancellationToken);

        IEnumerable<Role> rolesToAdd = roles.Except(existingRoles)
                                            .Select(role => Role.CreateForSeed(new RoleId(Guid.NewGuid()), restaurantId, role))
                                            .ToList();

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions)
                                                              .Select(permission => Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), restaurantId, permission))
                                                              .ToList();

        bool anyRolesToAdd = rolesToAdd.Any();
        bool anyPermissionsToAdd = permissionsToAdd.Any();

        if (anyRolesToAdd || anyPermissionsToAdd)
        {
            if (anyRolesToAdd)
            {
                await Roles.AddRangeAsync(rolesToAdd, cancellationToken);
            }

            if (anyPermissionsToAdd)
            {
                await Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
            }

            await SaveChangesAsync(cancellationToken);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Orders.Configurations"));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<PaymentId>().HaveConversion<PaymentIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
    }
}
