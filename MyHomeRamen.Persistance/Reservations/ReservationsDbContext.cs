using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Reservations;
using MyHomeRamen.Domain.Reservations.Database;
using MyHomeRamen.Persistance.Reservations.Converters;

namespace MyHomeRamen.Persistance.Reservations;

public class ReservationsDbContext : DbContext, IReservationsDbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<Table> Tables { get; set; }

    public DbSet<User> Users { get; set; }

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
    }
}
