using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

internal sealed class ShippingDetailsConfiguration : IEntityTypeConfiguration<ShippingDetails>
{
    public void Configure(EntityTypeBuilder<ShippingDetails> builder)
    {
        builder.ToTable("ShippingDetails");

        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.Property<BasketId>("BasketId").IsRequired();

        builder.HasOne<Basket>()
               .WithOne(b => b.ShippingDetails)
               .HasForeignKey<ShippingDetails>("BasketId");

        builder.OwnsOne(s => s.ShippingAddress, a =>
        {
            a.Property(p => p.Street).HasColumnName("ShippingAddress_Street");
            a.Property(p => p.Building).HasColumnName("ShippingAddress_Building");
            a.Property(p => p.Apartment).HasColumnName("ShippingAddress_Apartment");
            a.Property(p => p.City).HasColumnName("ShippingAddress_City");
            a.Property(p => p.ZipCode).HasColumnName("ShippingAddress_ZipCode");
        });
    }
}
