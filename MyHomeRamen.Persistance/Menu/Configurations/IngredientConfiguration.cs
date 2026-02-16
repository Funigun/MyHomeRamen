using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(IngredientConstants.MaxNameLength);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(IngredientConstants.MaxDescriptionLength);

        builder.Property(x => x.Price)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.HasMany(x => x.Categories)
               .WithMany()
               .UsingEntity(j => j.ToTable("IngredientCategories"));
    }
}
