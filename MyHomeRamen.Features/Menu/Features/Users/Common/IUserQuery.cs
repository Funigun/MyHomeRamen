using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IUserQuery
{
    Task<bool> Exists(UserId userId, CancellationToken cancellationToken);
}