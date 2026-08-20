using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Common.ClosingPeriod;
using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Persistance.Restaurants.Configurations;

internal sealed class ClosingPeriodConfiguration : IEntityTypeConfiguration<ClosingPeriod>
{
    public void Configure(EntityTypeBuilder<ClosingPeriod> builder)
    {
        builder.ToTable("RestaurantClosingPeriods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartTime)
                      .IsRequired();

        builder.Property(x => x.EndTime)
                      .IsRequired();

        builder.Property(x => x.Reason)
               .IsRequired()
               .HasMaxLength(ClosingPeriodConstants.MaxReasonLength);

        builder.Property(x => x.IsActive)
               .IsRequired();

    }
}
