using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
               .IsRequired();

        builder.Property(x => x.Price)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(x => x.Comment)
               .HasMaxLength(BasketConstants.MaxCommentLength);

        builder.HasOne(x => x.Product)
               .WithMany()
               .IsRequired();
    }
}
