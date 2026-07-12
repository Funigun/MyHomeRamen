using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Reservations.Features.Tables.Common;

public interface ITableRepository : IRepository<Table, TableId>
{
    ITableQuery Query();

    ITableSpecification Specification();
}
