using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IUserSpecification
{
    Task<User> ById(UserId userId, CancellationToken cancellationToken);
}