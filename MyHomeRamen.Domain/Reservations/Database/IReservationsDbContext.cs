using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Domain.Reservations.Database;

public interface IReservationsDbContext : IBaseDbContext
{
    DbSet<Booking> Bookings { get; }

    DbSet<Table> Tables { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }
}
