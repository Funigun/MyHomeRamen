using MyHomeRamen.Domain.Common.Basket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .IsRequired();

        builder.HasMany(x => x.Products)
            .WithOne();
    }
}
