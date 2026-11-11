using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.MenuCards;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public sealed class MenuCardConfiguration : IEntityTypeConfiguration<MenuCard>
{
    public void Configure(EntityTypeBuilder<MenuCard> builder)
    {
        builder.HasKey(card => card.Id);

        builder.Property(card => card.RestaurantId).IsRequired();
        builder.Property(card => card.Name)
            .HasMaxLength(MenuCardConstants.MaxNameLength)
            .IsRequired();
        builder.Property(card => card.IsActive).IsRequired();
        builder.Property(card => card.SortOrder).IsRequired();

        builder.HasIndex(card => new { card.RestaurantId, card.SortOrder });

        builder.HasMany(card => card.Sections)
            .WithOne(section => section.MenuCard)
            .HasForeignKey(section => section.MenuCardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
