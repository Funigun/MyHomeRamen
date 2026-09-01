using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Features.Common.Authorization;

public interface IAuthorizationService
{
    Task AuthorizeUser(HttpContext context, CancellationToken cancellationToken);

    Task<ICurrentUser> ImpersonateSystemAccount(CancellationToken cancellationToken);
}
