using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.GetDetails.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.GetDetails;

public sealed class GetDetailsHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetDetailsRequest, GetDetailsResponse>
{
    public async Task<GetDetailsResponse> Handle(GetDetailsRequest request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.GetByIdQuery(currentUser.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return user.ToGetDetailsResponse();
    }
}
