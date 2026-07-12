using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Reservations.Permissions;

namespace MyHomeRamen.Persistance.Reservations.Converters;

public class PermissionIdConverter : ValueConverter<PermissionId, Guid>
{
    public PermissionIdConverter() : base(id => id.Value, value => new PermissionId(value)) { }
}
