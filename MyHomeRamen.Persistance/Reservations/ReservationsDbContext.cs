using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Database;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Persistance.Reservations.Converters;

namespace MyHomeRamen.Persistance.Reservations;

public class ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : DbContext(options), IReservationsDbContext
{
    private readonly ICurrentUser _currentUser;

    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<Table> Tables { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

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

    public async Task Seed(Guid restaurantId, CancellationToken cancellationToken)
    {
        IEnumerable<string> roles = RoleConstants.AvailableRoles;
        IEnumerable<string> permissions = PermissionConstants.AvailablePermissions;

        HashSet<Permission> existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions.Select(p => p.Name))
                                                              .Select(permission => Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), permission))
                                                              .ToList();

        if (permissionsToAdd.Any())
        {
            await Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);
        HashSet<string> existingRoles = await Roles.AsNoTracking().Select(role => role.Name).ToHashSetAsync(cancellationToken);
        IEnumerable<Role> rolesToAdd = roles.Except(existingRoles)
                                            .Select(role => Role.CreateForSeed
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
        modelBuilder.HasDefaultSchema("reservations");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Reservations.Configurations"));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<BookingId>().HaveConversion<BookingIdConverter>();
        configurationBuilder.Properties<TableId>().HaveConversion<TableIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
    }
}
