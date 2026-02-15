using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Orders.Payments;

namespace MyHomeRamen.Persistance.Orders.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.HasMany(x => x.Products)
               .WithMany()
               .UsingEntity(j => j.ToTable("PaymentProducts"));
    }
}
