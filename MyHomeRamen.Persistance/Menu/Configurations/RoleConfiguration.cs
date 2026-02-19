using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxRoleNameLength);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.HasMany(x => x.Permissions)
               .WithMany()
               .UsingEntity(j => j.ToTable("RolePermissions"));
    }
}
