using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Configurations;

public class CompanyDetailsConfiguration : IEntityTypeConfiguration<CompanyDetails>
{
    public void Configure(EntityTypeBuilder<CompanyDetails> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(CompanyDetailsConstants.MaxNameLength);

        builder.Property(x => x.Description)
               .HasMaxLength(CompanyDetailsConstants.MaxDescriptionLength);

        builder.Property(x => x.LogoUrl)
               .HasMaxLength(CompanyDetailsConstants.MaxLogoUrlLength);

        builder.OwnsOne(x => x.BusinessDetails, businessDetails =>
        {
            businessDetails.Property(x => x.LegalName)
                           .HasColumnName(nameof(CompanyDetails.BusinessDetails.LegalName))
                           .IsRequired()
                           .HasMaxLength(CompanyDetailsConstants.MaxLegalNameLength);

            businessDetails.Property(x => x.TaxId)
                           .HasColumnName(nameof(CompanyDetails.BusinessDetails.TaxId))
                           .IsRequired()
                           .HasMaxLength(CompanyDetailsConstants.MaxTaxIdLength);
        });

        builder.HasMany(x => x.Media)
               .WithOne()
               .HasForeignKey("CompanyDetailsId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Media).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
