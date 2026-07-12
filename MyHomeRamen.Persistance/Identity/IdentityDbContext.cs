using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Persistance.Identity.Converters;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options), IIdentityDbContext
{
    private readonly RestaurantConfigurationProvider _restaurantConfiguration;
    private readonly ICurrentUser _currentUser;

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Address> Addresses { get; set; } = default!;

    public IUserRepository User => this;
    public IRoleRepository Role => this;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, RestaurantConfigurationProvider configFactory, ICurrentUser currentUser) : this(options)
    {
        _restaurantConfiguration = configFactory;
        _currentUser = currentUser;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
                    entry.Entity.CreatedBy = _currentUser.Id;
                    entry.Entity.SetRestaurantId(_restaurantConfiguration.RestaurantId);
                    break;
            }
        }

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Role> entry in ChangeTracker.Entries<Role>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.Id;
                    entry.Entity.SetRestaurantId(_restaurantConfiguration.RestaurantId);
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).ValueGeneratedNever();

            b.HasQueryFilter(u => u.RestaurantId == _restaurantConfiguration.RestaurantId);

            b.Property(u => u.RestaurantId)
             .IsRequired(true);

            b.Property(u => u.GuestId).IsRequired(false);
            b.Property(u => u.KeycloakUserId).IsRequired(false);
            b.HasIndex(u => u.GuestId).IsUnique().HasFilter("[GuestId] IS NOT NULL");

            b.HasMany<Address>()
             .WithMany()
             .UsingEntity("UserAddresses");

            b.HasMany(u => u.Roles)
             .WithMany()
             .UsingEntity("UserRoles");
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).ValueGeneratedNever();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    { 
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
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
        if (!await Roles.AnyAsync(cancellationToken))
        {
            IEnumerable<Role> roles = RoleConstants.AvailableRoles.Select(roleName => Domain.Identity.Roles.Role.CreateForSeed(roleName, $"{roleName} role"));

            await Roles.AddRangeAsync(roles, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    private UpdateSettersBuilder<TEntity> PrepareSettersBuilder<TEntity>(Dictionary<Expression<Func<TEntity, object>>, Expression> valuesToUpdate) where TEntity : class
    {
        UpdateSettersBuilder<TEntity> settersBuilder = new();

        foreach (KeyValuePair<Expression<Func<TEntity, object>>, Expression> kvp in valuesToUpdate)
        {
            settersBuilder.SetProperty(kvp.Key, kvp.Value);
        }

        return settersBuilder;
    }
}
