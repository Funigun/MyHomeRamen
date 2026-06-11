using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.PaymentGateway;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentGatewayConfiguration : IEntityTypeConfiguration<PaymentGateway>
{
    public void Configure(EntityTypeBuilder<PaymentGateway> builder)
    {
        builder.HasKey(gateway => gateway.Id);

        builder.Property(gateway => gateway.Name)
               .IsRequired()
               .HasMaxLength(PaymentGatewayConstants.MaxNameLength);
    }
}