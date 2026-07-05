using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : ITableSpecification
{
    async Task<Table> ITableSpecification.ByIdAsync(TableId tableId, CancellationToken cancellationToken)
        => await Set<Table>().FirstAsync(table => table.Id == tableId, cancellationToken);
}
