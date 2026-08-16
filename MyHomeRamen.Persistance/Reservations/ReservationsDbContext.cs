using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Reservations.Features.Abstractions;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;
using MyHomeRamen.Persistance.Reservations.Converters;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : DbContext(options), IReservationsDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<Table> Tables { get; set; }

    public IBookingRepository Booking => _serviceProvider.GetService<IBookingRepository>() ?? throw new InvalidOperationException("BookingRepository is not registered in the service provider.");

    public ITableRepository Table => _serviceProvider.GetService<ITableRepository>() ?? throw new InvalidOperationException("TableRepository is not registered in the service provider.");

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
        IRelationalDatabaseCreator? dbCreator = AccessorExtensions.GetService<IRelationalDatabaseCreator>(Database);

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
        modelBuilder.HasDefaultSchema("reservations");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Reservations.Configurations", StringComparison.OrdinalIgnoreCase));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<BookingId>().HaveConversion<BookingIdConverter>();
        configurationBuilder.Properties<TableId>().HaveConversion<TableIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
    }
}
