using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;
using MyHomeRamen.Features.Reservations.Features.Permissions.Common;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Features.Reservations.Features.Abstractions;

public interface IReservationsDbContext : IUnitOfWork
{
    IBookingRepository Booking { get; }

    ITableRepository Table { get; }

    IUserRepository User { get; }

    IRoleRepository Role { get; }

    IPermissionRepository Permission { get; }
}
