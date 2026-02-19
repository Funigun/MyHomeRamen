using MyHomeRamen.Domain.Common.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(UserConstants.MaxRoleNameLength);

        builder.HasMany(x => x.Permissions)
            .WithMany();
    }
}
