using MyHomeRamen.Domain.Common.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Permissions;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(UserConstants.MaxPermissionNameLength);
    }
}
