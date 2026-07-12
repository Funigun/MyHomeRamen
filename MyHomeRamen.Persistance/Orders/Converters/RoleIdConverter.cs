using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders.Roles;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class RoleIdConverter : ValueConverter<RoleId, Guid>
{
    public RoleIdConverter() : base(id => id.Value, value => new RoleId(value)) { }
}
