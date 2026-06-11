using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.PaymentMethod;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasKey(method => method.Id);

        builder.Property(method => method.Name)
            .IsRequired()
            .HasMaxLength(PaymentMethodConstants.MaxNameLength);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);

        builder.HasMany<PaymentChannel>()
               .WithOne()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
