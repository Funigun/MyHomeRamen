using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>
{
    IUserQuery Query();

    IUserSpecification Specification();
}
