using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public sealed class RestaurantIngredientPriceConfiguration : IEntityTypeConfiguration<RestaurantIngredientPrice>
{
    public void Configure(EntityTypeBuilder<RestaurantIngredientPrice> builder)
    {
        builder.HasKey(price => price.Id);

        builder.Property(price => price.RestaurantId).IsRequired();
        builder.Property(price => price.IngredientId).IsRequired();
        builder.Property(price => price.Price).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(price => new { price.RestaurantId, price.IngredientId }).IsUnique();

        builder.HasOne(price => price.Ingredient)
            .WithMany()
            .HasForeignKey(price => price.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
