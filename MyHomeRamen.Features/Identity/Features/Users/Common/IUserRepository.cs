using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Identity.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>
{
    void Update(User entity);

    void Update(IEnumerable<User> entities);

    IUserQuery Query();

    IUserSpecification Specification();
}
