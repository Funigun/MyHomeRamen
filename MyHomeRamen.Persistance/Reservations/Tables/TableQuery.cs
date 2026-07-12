using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : ITableQuery
{
    async Task<Table?> ITableQuery.ByIdAsync(TableId tableId, CancellationToken cancellationToken)
        => await Set<Table>().AsNoTracking().FirstOrDefaultAsync(table => table.Id == tableId, cancellationToken);
}
