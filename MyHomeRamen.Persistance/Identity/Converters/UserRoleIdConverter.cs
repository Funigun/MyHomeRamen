using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Persistance.Identity.Converters;

public sealed class UserRoleIdConverter() : ValueConverter<UserRoleId, Guid>(id => id.Value, value => new UserRoleId(value));
