using Microsoft.EntityFrameworkCore.Storage;

namespace MyHomeRamen.Api.Common.Domain;

public interface IBaseDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task RollbackTransaction(CancellationToken cancellationToken);

    Task<bool> EnsureCreated(CancellationToken cancellationToken);

    Task Migrate(CancellationToken cancellationToken);

    Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken);
}
