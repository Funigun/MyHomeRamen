using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations;

public readonly record struct BookingId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BookingId id) => id.Value;
    public static implicit operator BookingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
