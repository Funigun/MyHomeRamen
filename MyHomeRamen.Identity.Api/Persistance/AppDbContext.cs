using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Persistance;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Permission> Permissions { get; set; } = default!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("UserPermissions");
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("RolePermissions");
        });

        base.OnModelCreating(builder);
    }
}
