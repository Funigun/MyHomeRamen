using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.Users;


namespace MyHomeRamen.Persistance.Identity.Converters;

public class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() : base(id => id.Value, value => new UserId(value))
    {
    }
}
