using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders.Permissions;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class PermissionIdConverter : ValueConverter<PermissionId, Guid>
{
    public PermissionIdConverter() : base(id => id.Value, value => new PermissionId(value)) { }
}
