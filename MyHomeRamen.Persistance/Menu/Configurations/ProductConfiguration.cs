using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(ProductConstants.MaxNameLength);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(ProductConstants.MaxDescriptionLength);

        builder.Property(x => x.Price)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);

        builder.HasMany(x => x.BaseIngredients)
               .WithMany()
               .UsingEntity(j => j.ToTable("ProductBaseIngredients"));

        builder.HasMany(x => x.CustomIngredients)
               .WithMany()
               .UsingEntity(j => j.ToTable("ProductCustomIngredients"));

        builder.HasMany(x => x.Categories)
               .WithMany()
               .UsingEntity(j => j.ToTable("ProductCategories"));
    }
}
