using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Orders.Products;

namespace MyHomeRamen.Persistance.Orders.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(x => x.Name)
       .IsRequired()
       .HasMaxLength(ProductConstants.MaxNameLength);

        builder.Property(x => x.OriginalPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.CalculatedPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.HasMany(x => x.BaseIngredients)
               .WithMany()
               .UsingEntity(j => j.ToTable("ProductBaseIngredients"));

        builder.HasMany(x => x.CustomIngredients)
               .WithMany()
               .UsingEntity(j => j.ToTable("ProductCustomIngredients"));
    }
}
