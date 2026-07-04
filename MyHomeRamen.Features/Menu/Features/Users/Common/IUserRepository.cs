using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>, IUserQuery, IUserSpecification
{
    IUserQuery Query();

    IUserSpecification Specification();
}
