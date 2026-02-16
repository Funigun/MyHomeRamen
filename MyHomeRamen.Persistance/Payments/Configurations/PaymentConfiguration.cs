using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.Payment;
using MyHomeRamen.Domain.Payments.Payments;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReferenceId)
               .IsRequired();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(PaymentConstants.MaxNameLength);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);
    }
}

