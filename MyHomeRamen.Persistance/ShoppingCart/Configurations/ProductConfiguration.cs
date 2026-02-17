using MyHomeRamen.Domain.Common.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(ProductConstants.MaxNameLength);

        builder.Property(x => x.Description)
            .HasMaxLength(ProductConstants.MaxDescriptionLength);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(ProductConstants.MaxImageUrlLength);

        builder.HasMany(x => x.BaseIngredients)
            .WithMany();

        builder.HasMany(x => x.CustomIngredients)
            .WithMany();
    }
}
