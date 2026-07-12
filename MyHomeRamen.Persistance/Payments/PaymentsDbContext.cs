using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Domain.Payments.Roles;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Payments.Features.Orders.Common;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;
using MyHomeRamen.Features.Payments.Features.Roles.Common;
using MyHomeRamen.Features.Payments.Features.Users.Common;
using MyHomeRamen.Persistance.Payments.Converters;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options), IPaymentsDbContext
{
    private readonly ICurrentUser _currentUser = default!;

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<PaymentChannel> PaymentChannels { get; set; }

    public DbSet<PaymentGateway> PaymentGateways { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public IPaymentMethodRepository PaymentMethod => this;

    public IPaymentChannelRepository PaymentChannel => this;

    public IPaymentGatewayRepository PaymentGateway => this;

    public IOrderRepository Order => this;

    public IUserRepository User => this;

    public IRoleRepository Role => this;

    public IPermissionRepository Permission => this;

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

    public async Task Seed(CancellationToken cancellationToken)
    {
        IEnumerable<string> roles = RoleConstants.AvailableRoles;
        IEnumerable<string> permissions = PermissionConstants.AvailablePermissions;

        HashSet<Permission> existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions.Select(p => p.Name))
                                                              .Select(permission => Domain.Payments.Permissions.Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), permission))
                                                              .ToList();

        if (permissionsToAdd.Any())
        {
            await Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);
        HashSet<string> existingRoles = await Roles.AsNoTracking().Select(role => role.Name).ToHashSetAsync(cancellationToken);
        IEnumerable<Role> rolesToAdd = roles.Except(existingRoles)
                                            .Select(role => Domain.Payments.Roles.Role.CreateForSeed
                                                        (
                                                            new RoleId(Guid.NewGuid()),
                                                            role,
                                                            existingPermissions.Where(p => RoleConstants.DefaultPermissions[role].Contains(p.Name))
                                                                               .ToList()
                                                        )
                                                   );

        if (rolesToAdd.Any())
        {
            await Roles.AddRangeAsync(rolesToAdd, cancellationToken);
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
        configurationBuilder.Properties<PaymentMethodId>().HaveConversion<PaymentMethodIdConverter>();
        configurationBuilder.Properties<PaymentChannelId>().HaveConversion<PaymentChannelIdConverter>();
        configurationBuilder.Properties<PaymentGatewayId>().HaveConversion<PaymentGatewayIdConverter>();
        configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
    }

    private UpdateSettersBuilder<TEntity> PrepareSettersBuilder<TEntity>(Dictionary<Expression<Func<TEntity, object>>, Expression> valuesToUpdate) where TEntity : class
    {
        UpdateSettersBuilder<TEntity> settersBuilder = new UpdateSettersBuilder<TEntity>();

        foreach (KeyValuePair<Expression<Func<TEntity, object>>, Expression> kvp in valuesToUpdate)
        {
            settersBuilder.SetProperty(kvp.Key, kvp.Value);
        }

        return settersBuilder;
    }
}
