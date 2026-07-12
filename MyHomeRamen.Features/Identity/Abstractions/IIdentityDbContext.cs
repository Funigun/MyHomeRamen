using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Features.Identity.Abstractions;

public interface IIdentityDbContext : IUnitOfWork
{
    IUserRepository User { get; }

    IRoleRepository Role { get; }
}
