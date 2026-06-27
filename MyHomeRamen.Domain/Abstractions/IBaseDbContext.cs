using Microsoft.EntityFrameworkCore.Storage;

namespace MyHomeRamen.Domain.Abstractions;

public interface IBaseDbContext : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task RollbackTransaction(CancellationToken cancellationToken);

    Task<bool> EnsureCreated(CancellationToken cancellationToken);

    Task Migrate(CancellationToken cancellationToken);

    Task Seed(Guid restaurantId, CancellationToken cancellationToken);

    Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken);
}
