using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(CompanyConstants.MaxNameLength);

        builder.Property(x => x.NormalizedName)
               .IsRequired()
               .HasMaxLength(CompanyConstants.MaxNameLength);

        builder.HasIndex(x => x.NormalizedName).IsUnique();

        builder.Property(x => x.Description)
               .HasMaxLength(CompanyConstants.MaxDescriptionLength);

        builder.Property(x => x.LogoUrl)
               .HasMaxLength(CompanyConstants.MaxLogoUrlLength);

        builder.OwnsOne(x => x.BusinessDetails, businessDetails =>
        {
            businessDetails.Property(x => x.LegalName)
                           .HasColumnName(nameof(Company.BusinessDetails.LegalName))
                           .IsRequired()
                           .HasMaxLength(CompanyConstants.MaxLegalNameLength);

            businessDetails.Property(x => x.TaxId)
                           .HasColumnName(nameof(Company.BusinessDetails.TaxId))
                           .IsRequired()
                           .HasMaxLength(CompanyConstants.MaxTaxIdLength);
        });

        builder.HasMany(x => x.Media)
               .WithOne()
               .HasForeignKey("CompanyId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Media).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
