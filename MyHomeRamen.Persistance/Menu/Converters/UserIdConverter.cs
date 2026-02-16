using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Persistance.Menu.Converters;

public class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() : base(id => id.Value, value => new UserId(value))
    {
    }
}
