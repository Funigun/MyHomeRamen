using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.HasMany(x => x.Roles)
               .WithMany();

        builder.HasMany(x => x.Permissions)
               .WithMany();

        builder.Property(x => x.IsGuest)
               .IsRequired()
               .HasDefaultValue(false);
    }
}
