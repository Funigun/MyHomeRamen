using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Identity.Api.Application;
using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Persistance;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    private RestaurantConfigurationProvider RestaurantConfiguration { get; init; }

    public DbSet<Permission> Permissions { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options, RestaurantConfigurationProvider configFactory) : base(options)
    {
        RestaurantConfiguration = configFactory;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.HasQueryFilter(u => u.RestaurantId == RestaurantConfiguration.RestaurantId);

            b.Property(u => u.RestaurantId)
             .IsRequired(true);

            b.Ignore(u => u.LockoutEnd);
            b.Ignore(u => u.TwoFactorEnabled);
            b.Ignore(u => u.PhoneNumberConfirmed);
            b.Ignore(u => u.ConcurrencyStamp);
            b.Ignore(u => u.SecurityStamp);
            b.Ignore(u => u.NormalizedEmail);
            b.Ignore(u => u.LockoutEnabled);

            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("UserPermissions");
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");

            b.Property(p => p.RestaurantId)
             .IsRequired(true);

            b.HasQueryFilter(u => u.RestaurantId == RestaurantConfiguration.RestaurantId);

            b.Ignore(u => u.NormalizedName);
            b.Ignore(u => u.ConcurrencyStamp);

            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("RolePermissions");
        });

        builder.Entity<Permission>(b =>
        {
            b.ToTable("Permissions");
            b.HasQueryFilter(p => p.RestaurantId == RestaurantConfiguration.RestaurantId);
            b.Property(p => p.RestaurantId).IsRequired(true);
            b.Property(p => p.Description).HasMaxLength(500);
        });

        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Add(_ => new GuidFinalizingConvention());
    }
}
