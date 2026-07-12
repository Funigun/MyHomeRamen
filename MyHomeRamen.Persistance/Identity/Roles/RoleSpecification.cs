using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class IdentityDbContext : IRoleSpecification
{
    public Task<Role> ByName(string roleName, CancellationToken cancellationToken)
        => Roles.FirstAsync(role => role.Name.ToLower() == roleName.ToLower(), cancellationToken);
}
