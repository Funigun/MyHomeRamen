using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Common.GuidConvention;

namespace MyHomeRamen.Persistance.Users;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext<User, Role, Guid>(options), IUsersDbContext
{
    private readonly RestaurantConfigurationProvider _restaurantConfiguration = default!;
    private readonly ICurrentUser _currentUser = default!;

    public DbSet<Address> Addresses { get; set; } = default!;

    public UsersDbContext(DbContextOptions<UsersDbContext> options, RestaurantConfigurationProvider configFactory, ICurrentUser currentUser) : this(options)
    {
        _restaurantConfiguration = configFactory;
        _currentUser = currentUser;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateEntities()
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entry in ChangeTracker.Entries<User>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetRestaurantId(_currentUser.RestaurantId);
                    break;
            }
        }

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Role> entry in ChangeTracker.Entries<Role>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetRestaurantId(_currentUser.RestaurantId);
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.HasQueryFilter(u => u.RestaurantId == _restaurantConfiguration.RestaurantId);

            b.Property(u => u.RestaurantId)
             .IsRequired(true);

            b.Ignore(u => u.LockoutEnd);
            b.Ignore(u => u.TwoFactorEnabled);
            b.Ignore(u => u.PhoneNumberConfirmed);
            b.Ignore(u => u.ConcurrencyStamp);
            b.Ignore(u => u.SecurityStamp);
            b.Ignore(u => u.NormalizedEmail);
            b.Ignore(u => u.LockoutEnabled);

            b.HasMany<Address>()
             .WithMany()
             .UsingEntity("UserAddresses");
        });

        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Add(_ => new GuidFinalizingConvention());
    }

    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransaction(CancellationToken cancellationToken)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
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

    public Task Seed(Guid restaurantId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }
}
