using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.GetDetails.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.GetDetails;

public sealed class GetDetailsHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetDetailsRequest, GetDetailsResponse>
{
    public async Task<GetDetailsResponse> Handle(GetDetailsRequest request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.KeycloakUserId == currentUser.Id, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return user.ToGetDetailsResponse();
    }
}
