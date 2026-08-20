using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Persistance.Restaurants.Converters;

public class ClosingPeriodIdConverter : ValueConverter<ClosingPeriodId, Guid>
{
    public ClosingPeriodIdConverter() : base(id => id.Value, value => new ClosingPeriodId(value))
    {
    }
}
