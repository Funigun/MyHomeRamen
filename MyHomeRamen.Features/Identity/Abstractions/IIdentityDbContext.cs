using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Features.CompanyMemberships.Common;

namespace MyHomeRamen.Features.Identity.Abstractions;

public interface IIdentityDbContext : IUnitOfWork
{
    IUserRepository User { get; }

    ICompanyMembershipRepository CompanyMembership { get; }

    IRoleRepository Role { get; }

    IPermissionRepository Permission { get; }
}
