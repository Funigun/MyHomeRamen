using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Features.Permissions.Common;

public interface IPermissionLoader
{
    Task<IEnumerable<Permission>> All(CancellationToken cancellationToken);

    Task<Permission?> ByModuleAndName(string module, string name, CancellationToken cancellationToken);
}
