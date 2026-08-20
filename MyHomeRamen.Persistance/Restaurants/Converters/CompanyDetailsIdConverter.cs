using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Converters;

public class CompanyDetailsIdConverter : ValueConverter<CompanyDetailsId, Guid>
{
    public CompanyDetailsIdConverter() : base(id => id.Value, value => new CompanyDetailsId(value))
    {
    }
}
