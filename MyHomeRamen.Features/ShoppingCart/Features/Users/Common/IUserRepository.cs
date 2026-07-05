using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>
{
    IUserQuery Query();

    IUserSpecification Specification();
}
