using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Persistance;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Permission> Permissions { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.Property(u => u.RestgaurantId)
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

            b.Ignore(u => u.NormalizedName);
            b.Ignore(u => u.ConcurrencyStamp);

            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("RolePermissions");
        });

        base.OnModelCreating(builder);
    }
}
