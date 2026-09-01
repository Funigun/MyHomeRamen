using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Persistance.Identity.Converters;

public sealed class PermissionIdConverter : ValueConverter<PermissionId, Guid>
{
    public PermissionIdConverter() : base(id => id.Value, value => new PermissionId(value))
    {
    }
}
