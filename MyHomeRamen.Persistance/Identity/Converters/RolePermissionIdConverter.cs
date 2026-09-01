using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Persistance.Identity.Converters;

public class RolePermissionIdConverter : ValueConverter<RolePermissionId, Guid>
{
    public RolePermissionIdConverter() : base(id => id.Value, value => new RolePermissionId(value))
    {
    }
}
