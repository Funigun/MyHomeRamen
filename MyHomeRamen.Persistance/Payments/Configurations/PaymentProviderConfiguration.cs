using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.PaymentProvider;
using MyHomeRamen.Domain.Payments.PaymentProviders;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentProviderConfiguration : IEntityTypeConfiguration<PaymentProvider>
{
    public void Configure(EntityTypeBuilder<PaymentProvider> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(PaymentProviderConstants.MaxNameLength);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);

        builder.HasMany(x => x.Payments)
               .WithMany()
               .UsingEntity(j => j.ToTable("PaymentProviderPayments"));
    }
}
