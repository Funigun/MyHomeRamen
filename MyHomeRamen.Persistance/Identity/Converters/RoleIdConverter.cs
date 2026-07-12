using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Persistance.Identity.Converters;

public class RoleIdConverter : ValueConverter<RoleId, Guid>
{
    public RoleIdConverter() : base(id => id.Value, value => new RoleId(value))
    {
    }
}
