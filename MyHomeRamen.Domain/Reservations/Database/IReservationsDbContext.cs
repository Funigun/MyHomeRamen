using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations.Database;

public interface IReservationsDbContext : IBaseDbContext
{
    DbSet<Booking> Bookings { get; }

    DbSet<Table> Tables { get; }

    DbSet<User> Users { get; }
}
