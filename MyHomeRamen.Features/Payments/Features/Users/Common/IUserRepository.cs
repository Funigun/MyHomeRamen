using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Users.Common;

public interface IUserRepository : IRepository<User, UserId>, IUserQuery, IUserSpecification
{
    IUserQuery Query();

    IUserSpecification Specification();
}
