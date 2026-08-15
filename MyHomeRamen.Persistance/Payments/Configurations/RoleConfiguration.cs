using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Payments.Roles;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxRoleNameLength);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasMany(x => x.Permissions)
               .WithMany()
               .UsingEntity(j => j.ToTable("RolePermissions"));
    }
}
