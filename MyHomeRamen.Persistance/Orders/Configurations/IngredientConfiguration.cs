using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Orders.Ingredients;

namespace MyHomeRamen.Persistance.Orders.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(IngredientConstants.MaxNameLength);

        builder.Property(x => x.OriginalPrice)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.CalculatedPrice)
               .IsRequired()
               .HasPrecision(18, 2);
    }
}
