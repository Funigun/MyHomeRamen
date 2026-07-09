namespace MyHomeRamen.Features.Common.Repository;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> EnsureCreated(CancellationToken cancellationToken);

    Task Migrate(CancellationToken cancellationToken);

    Task Seed(CancellationToken cancellationToken);

    Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken);
}
