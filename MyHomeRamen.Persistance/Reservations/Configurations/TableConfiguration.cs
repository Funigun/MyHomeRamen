using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeRamen.Domain.Reservations.Tables;

namespace MyHomeRamen.Persistance.Reservations.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RestaurantId)
               .IsRequired();
    }
}
