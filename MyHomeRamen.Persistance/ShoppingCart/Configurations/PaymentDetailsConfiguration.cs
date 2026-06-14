using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;

namespace MyHomeRamen.Persistance.ShoppingCart.Configurations;

internal sealed class PaymentDetailsConfiguration : IEntityTypeConfiguration<PaymentDetails>
{
    public void Configure(EntityTypeBuilder<PaymentDetails> builder)
    {
        builder.ToTable("PaymentDetails");
        
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.Property<BasketId>("BasketId").IsRequired();

        builder.HasOne<Basket>()
               .WithOne(b => b.PaymentDetails)
               .HasForeignKey<PaymentDetails>("BasketId");
    }
}
