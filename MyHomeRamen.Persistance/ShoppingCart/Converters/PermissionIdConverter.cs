using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.ShoppingCart.Converters;

public class PermissionIdConverter : ValueConverter<PermissionId, Guid>
{
    public PermissionIdConverter() : base(id => id.Value, value => new PermissionId(value)) { }
}
