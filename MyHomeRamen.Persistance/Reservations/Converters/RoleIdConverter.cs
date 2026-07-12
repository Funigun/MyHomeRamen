using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Reservations.Roles;

namespace MyHomeRamen.Persistance.Reservations.Converters;

public class RoleIdConverter : ValueConverter<RoleId, Guid>
{
    public RoleIdConverter() : base(id => id.Value, value => new RoleId(value)) { }
}
