using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Orders.Orders;

namespace MyHomeRamen.Persistance.Orders.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();

        builder.Property(order => order.Type)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(order => order.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(x => x.TotalOriginalAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.TotalCalculatedAmount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.OwnsOne(x => x.DeliveryAddress);

        builder.HasOne(x => x.User)
               .WithMany()
               .IsRequired();

        builder.HasMany(x => x.Products)
               .WithOne()
               .IsRequired();

        builder.HasMany(x => x.Payments)
               .WithOne(p => p.Order)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
