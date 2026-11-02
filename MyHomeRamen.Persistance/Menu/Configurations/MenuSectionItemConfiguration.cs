using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.MenuSectionItems;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public sealed class MenuSectionItemConfiguration : IEntityTypeConfiguration<MenuSectionItem>
{
    public void Configure(EntityTypeBuilder<MenuSectionItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.MenuSectionId).IsRequired();
        builder.Property(item => item.RestaurantProductOfferId).IsRequired();
        builder.Property(item => item.SortOrder).IsRequired();

        builder.HasIndex(item => new { item.MenuSectionId, item.RestaurantProductOfferId })
            .IsUnique();

        builder.HasOne(item => item.RestaurantProductOffer)
            .WithMany()
            .HasForeignKey(item => item.RestaurantProductOfferId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
