using MyHomeRamen.Domain.Reservations.Tables;

namespace MyHomeRamen.Features.Reservations.Features.Tables.Common;

public interface ITableSpecification
{
    Task<Table> ByIdAsync(TableId tableId, CancellationToken cancellationToken);
}
