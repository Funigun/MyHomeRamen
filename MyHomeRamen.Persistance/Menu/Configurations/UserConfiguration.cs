using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.HasMany(x => x.Roles)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserRoles"));

        builder.HasMany(x => x.Permissions)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserPermissions"));

        builder.HasMany(x => x.FavoriteProducts)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserFavoriteProducts"));
    }
}
