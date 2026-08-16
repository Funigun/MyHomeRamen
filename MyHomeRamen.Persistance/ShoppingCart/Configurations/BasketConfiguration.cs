using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.HasMany(x => x.Items)
               .WithOne();

        builder.Property(x => x.Status)
               .HasConversion<int>()
               .IsRequired();
    }
}
