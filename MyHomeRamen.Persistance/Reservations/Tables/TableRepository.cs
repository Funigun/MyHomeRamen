using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class TableRepository(ReservationsDbContext reservationsDbContext, ICacheService cacheService)
    : BaseRepository<Table, TableId>(reservationsDbContext, cacheService), ITableRepository
{
    public ITableQuery Query() => this;

    public ITableSpecification Specification() => this;
}