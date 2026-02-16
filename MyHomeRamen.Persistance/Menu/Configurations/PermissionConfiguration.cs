using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(UserConstants.MaxPermissionNameLength);

        builder.Property(x => x.RestaurantId)
               .IsRequired();
    }
}
