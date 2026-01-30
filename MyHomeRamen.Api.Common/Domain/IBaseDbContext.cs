using Microsoft.EntityFrameworkCore.Storage;

namespace MyHomeRamen.Api.Common.Domain;

public interface IBaseDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task RollbackTransaction(CancellationToken cancellationToken);
}
