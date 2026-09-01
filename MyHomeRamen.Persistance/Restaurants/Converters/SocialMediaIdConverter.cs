using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Persistance.Restaurants.Converters;

public class SocialMediaIdConverter : ValueConverter<SocialMediaId, Guid>
{
    public SocialMediaIdConverter() : base(id => id.Value, value => new SocialMediaId(value))
    {
    }
}
