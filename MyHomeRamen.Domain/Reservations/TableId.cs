using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations;

public readonly record struct TableId(Guid Value) : IEntityId
{
    public static implicit operator Guid(TableId id) => id.Value;
    public static implicit operator TableId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
