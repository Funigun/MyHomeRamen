using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.PaymentGroup;
using MyHomeRamen.Domain.Payments.PaymentGroups;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentGroupConfiguration : IEntityTypeConfiguration<PaymentGroup>
{
    public void Configure(EntityTypeBuilder<PaymentGroup> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(PaymentGroupConstants.MaxNameLength);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);

         builder.HasMany(x => x.PaymentProviders)
                .WithMany()
                .UsingEntity(j => j.ToTable("PaymentGroupProviders"));
    }
}
