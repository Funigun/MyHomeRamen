using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PermissionIdConverter : ValueConverter<PermissionId, Guid>
{
    public PermissionIdConverter() : base(id => id.Value, value => new PermissionId(value))
    {
    }
}
