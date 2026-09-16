using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Converters;

public class CompanyIdConverter : ValueConverter<CompanyId, Guid>
{
    public CompanyIdConverter() : base(id => id.Value, value => new CompanyId(value))
    {
    }
}
