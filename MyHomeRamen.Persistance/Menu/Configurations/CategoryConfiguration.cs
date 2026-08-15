using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Persistance.Menu.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(category => category.Name)
               .HasMaxLength(CategoryConstants.MaxNameLength)
               .IsRequired();

        builder.Property(category => category.SortOrder)
               .IsRequired();

        builder.Property(category => category.CategoryType)
               .HasConversion<string>()
               .IsRequired();
    }
}
