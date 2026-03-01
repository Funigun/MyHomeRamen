using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentGroups;
using MyHomeRamen.Domain.Payments.PaymentProviders;
using MyHomeRamen.Domain.Payments.Payments;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Persistance.Payments.Converters;

namespace MyHomeRamen.Persistance.Payments;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options), IPaymentsDbContext
{
    private readonly ICurrentUser _currentUser;

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<PaymentProvider> PaymentProviders { get; set; }

    public DbSet<PaymentGroup> PaymentGroups { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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
                    entry.Entity.CreatedBy = _currentUser.Id.ToString();
                    entry.Entity.CreatedOn = currentDateTime;
                    entry.Entity.SetRestaurantId(_currentUser.RestaurantId);
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.Id.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }

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
                                            .Select(role => Role.CreateForSeed(new RoleId(Guid.NewGuid()), role))
                                            .ToList();

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions)
                                                              .Select(permission => Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), permission))
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

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
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
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
        configurationBuilder.Properties<PaymentProviderId>().HaveConversion<PaymentProviderIdConverter>();
        configurationBuilder.Properties<PaymentGroupId>().HaveConversion<PaymentGroupIdConverter>();
    }
}
