using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Reservations.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>
{
    IUserQuery Query();

    IUserSpecification Specification();
}
