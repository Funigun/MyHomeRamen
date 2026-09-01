using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Persistance.Identity.Converters;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Persistance.Identity;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options), IIdentityDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public IUserRepository User => _serviceProvider.GetService<IUserRepository>() ?? throw new InvalidOperationException("UserRepository is not registered in the service provider.");
    public IRoleRepository Role => _serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("RoleRepository is not registered in the service provider.");
    public IPermissionRepository Permission => _serviceProvider.GetService<IPermissionRepository>() ?? throw new InvalidOperationException("PermissionRepository is not registered in the service provider.");

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options, currentUser)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateEntities()
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.UserId.ToString();
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

            b.Property(u => u.GuestId).IsRequired(false);
            b.Property(u => u.KeycloakUserId).IsRequired(false);
            b.HasIndex(u => u.GuestId).IsUnique().HasFilter("[GuestId] IS NOT NULL");

            b.OwnsMany(u => u.Addresses, owned =>
            {
                owned.Property<Guid>(nameof(Address.Id))
                     .HasColumnName(nameof(Address.Id))
                     .ValueGeneratedNever();
            });

        b.HasMany(u => u.Roles)
             .WithMany()
             .UsingEntity("UserRoles");
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).ValueGeneratedNever();
            b.HasMany(role => role.RolePermissions)
             .WithOne()
             .HasForeignKey(rolePermission => rolePermission.RoleId);
        });

        modelBuilder.Entity<Permission>(b =>
        {
            b.ToTable("Permissions");
            b.HasKey(permission => permission.Id);
            b.Property(permission => permission.Id).ValueGeneratedNever();
            b.Property(permission => permission.Name).IsRequired();
            b.Property(permission => permission.Description).IsRequired();
            b.Property(permission => permission.Module).IsRequired();
            b.Property(permission => permission.IsResourceScoped).IsRequired();
            b.HasIndex(permission => new { permission.Module, permission.Name }).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(b =>
        {
            b.ToTable("RolePermissions");
            b.HasKey(rp => rp.Id);
            b.Property(rp => rp.Id).ValueGeneratedNever();
            b.Property(rp => rp.RoleId).IsRequired();
            b.Property(rp => rp.PermissionId).IsRequired();
            b.HasOne<Permission>()
             .WithMany()
             .HasForeignKey(rp => rp.PermissionId);
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    { 
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
        configurationBuilder.Properties<RolePermissionId>().HaveConversion<RolePermissionIdConverter>();
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
            IEnumerable<Role> roles = RoleConstants.AvailableRoles.Select(roleName => Domain.Identity.Roles.Role.Create(roleName, $"{roleName} role"));

            await Roles.AddRangeAsync(roles, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }
}
