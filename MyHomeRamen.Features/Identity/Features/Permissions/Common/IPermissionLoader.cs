using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Features.Permissions.Common;

public interface IPermissionLoader
{
    Task<Permission?> ByModuleAndName(string module, string name, CancellationToken cancellationToken);
}
