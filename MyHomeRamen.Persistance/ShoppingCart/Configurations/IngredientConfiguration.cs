using MyHomeRamen.Domain.Common.Ingredient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(IngredientConstants.MaxNameLength);

        builder.Property(x => x.Description)
            .HasMaxLength(IngredientConstants.MaxDescriptionLength);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);
    }
}
