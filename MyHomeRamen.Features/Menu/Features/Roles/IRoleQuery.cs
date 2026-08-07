using MyHomeRamen.Domain.Menu.Roles;

namespace MyHomeRamen.Features.Menu.Features.Roles;

public interface IRoleQuery
{
    Task<IEnumerable<Role>> GetAll(CancellationToken cancellationToken);
}
