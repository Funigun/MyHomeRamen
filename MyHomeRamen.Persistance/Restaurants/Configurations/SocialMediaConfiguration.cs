using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.SocialMedia;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Configurations;

internal sealed class SocialMediaConfiguration : IEntityTypeConfiguration<SocialMedia>
{
    public void Configure(EntityTypeBuilder<SocialMedia> builder)
    {
        builder.ToTable("CompanySocialMedia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(SocialMediaConstants.MaxNameLength);

        builder.Property(x => x.LogoUrl)
               .IsRequired()
               .HasMaxLength(SocialMediaConstants.MaxLogoUrlLength);

        builder.Property(x => x.Url)
               .IsRequired()
               .HasMaxLength(SocialMediaConstants.MaxUrlLength);
    }
}
