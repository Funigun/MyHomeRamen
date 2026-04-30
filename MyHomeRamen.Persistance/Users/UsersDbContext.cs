using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Common.GuidConvention;

namespace MyHomeRamen.Persistance.Users;

public class UsersDbContext : IdentityDbContext<User, Role, Guid>, IUsersDbContext
{
    private readonly RestaurantConfigurationProvider _restaurantConfiguration;
    private readonly ICurrentUser _currentUser;

    public DbSet<Address> Addresses { get; set; } = default!;

    public UsersDbContext(DbContextOptions<UsersDbContext> options, RestaurantConfigurationProvider configFactory, ICurrentUser currentUser) : base(options)
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
                    entry.Entity.SetRestaurantId(_restaurantConfiguration.RestaurantId);
                    break;
            }
        }

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Role> entry in ChangeTracker.Entries<Role>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetRestaurantId(_restaurantConfiguration.RestaurantId);
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.HasQueryFilter(u => u.RestaurantId == _restaurantConfiguration.RestaurantId);

            b.Property(u => u.RestaurantId)
             .IsRequired(true);

            b.Property(u => u.GuestId).IsRequired(false);
            b.Property(u => u.KeycloakUserId).IsRequired(false);
            b.HasIndex(u => u.GuestId).IsUnique().HasFilter("[GuestId] IS NOT NULL");

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

        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
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

    public Task Seed(Guid restaurantId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }
}
