namespace MyHomeRamen.Features.Identity.ExternalApi;

public interface IPermissionCatalogSynchronizer
{
    Task Synchronize(CancellationToken cancellationToken);
}
