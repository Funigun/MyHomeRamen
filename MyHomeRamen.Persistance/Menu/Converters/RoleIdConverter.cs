using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Persistance.Menu.Converters;

public class RoleIdConverter : ValueConverter<RoleId, Guid>
{
    public RoleIdConverter() : base(id => id.Value, value => new RoleId(value))
    {
    }
}
