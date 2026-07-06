using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Orders.Roles;

namespace MyHomeRamen.Persistance.Orders.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(UserConstants.MaxRoleNameLength);

        builder.HasMany(x => x.Permissions)
               .WithMany();
    }
}
