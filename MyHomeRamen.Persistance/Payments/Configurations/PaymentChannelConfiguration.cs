using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.PaymentChannel;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Persistance.Payments.Configurations;

public class PaymentChannelConfiguration : IEntityTypeConfiguration<PaymentChannel>
{
    public void Configure(EntityTypeBuilder<PaymentChannel> builder)
    {
        builder.HasKey(channel => channel.Id);

        builder.Property(channel => channel.Name)
               .IsRequired()
               .HasMaxLength(PaymentChannelConstants.MaxNameLength);

        builder.Property(channel => channel.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);

        builder.HasOne<PaymentGateway>()
               .WithMany()
               .OnDelete(DeleteBehavior.ClientNoAction);
    }
}