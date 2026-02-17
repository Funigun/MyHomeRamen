using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Database;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Persistance.Reservations.Converters;

namespace MyHomeRamen.Persistance.Reservations;

public class ReservationsDbContext : DbContext, IReservationsDbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<Table> Tables { get; set; }

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

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlAsync(sql, cancellationToken);
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
