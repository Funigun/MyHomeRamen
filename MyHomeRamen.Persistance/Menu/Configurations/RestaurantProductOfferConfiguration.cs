using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.Offers;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public sealed class RestaurantProductOfferConfiguration : IEntityTypeConfiguration<RestaurantProductOffer>
{
    public void Configure(EntityTypeBuilder<RestaurantProductOffer> builder)
    {
        builder.HasKey(offer => offer.Id);

        builder.Property(offer => offer.CompanyId).IsRequired();
        builder.Property(offer => offer.RestaurantId).IsRequired();
        builder.Property(offer => offer.ProductId).IsRequired();
        builder.Property(offer => offer.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(offer => offer.Status).HasConversion<string>().IsRequired();

        builder.HasIndex(offer => new { offer.RestaurantId, offer.ProductId }).IsUnique();
        builder.HasIndex(offer => new { offer.CompanyId, offer.Status });

        builder.HasOne(offer => offer.Product)
            .WithMany()
            .HasForeignKey(offer => offer.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
