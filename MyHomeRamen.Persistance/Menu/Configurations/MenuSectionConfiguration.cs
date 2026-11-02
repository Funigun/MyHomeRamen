using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Menu.MenuSections;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public sealed class MenuSectionConfiguration : IEntityTypeConfiguration<MenuSection>
{
    public void Configure(EntityTypeBuilder<MenuSection> builder)
    {
        builder.HasKey(section => section.Id);

        builder.Property(section => section.MenuCardId).IsRequired();
        builder.Property(section => section.Name)
            .HasMaxLength(MenuSectionConstants.MaxNameLength)
            .IsRequired();
        builder.Property(section => section.SortOrder).IsRequired();

        builder.HasMany(section => section.Items)
            .WithOne(item => item.MenuSection)
            .HasForeignKey(item => item.MenuSectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
